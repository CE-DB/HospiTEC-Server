using HospiTec_Server.Logic.Graphql.Types;
using HospiTec_Server.Logic.Graphql.Types.Inputs;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using HospiTec_Server.database;
using HospiTec_Server.database.DBModels;
using HotChocolate.AspNetCore.Authorization;

namespace HospiTec_Server.Logic.Graphql
{
    public class Mutation
    {
        /// <summary>
        /// This is for make custom errors and bypass the database code an error
        /// </summary>
        /// <param name="sqlException">The exception object from Database.</param>
        /// <param name="message">The message for user.</param>
        /// <param name="args"If the message has format you can insert variables here></param>
        /// <returns>The error object to throw.</returns>
        private IError CustomErrorBuilder(PostgresException sqlException,
            string message,
            params object[] args)
        {
            return ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(sqlException.SqlState)
                           .SetExtension("DatabaseMessage", sqlException.Message)
                           .Build();
        }

        /// <summary>
        /// This is for make customized errors, using custom code and messages
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private IError CustomErrorBuilder(string code,
            string message,
            params object[] args)
        {
            return ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(code)
                           .SetExtension("DatabaseMessage", "NO ERROR")
                           .Build();
        }

        /// <summary>
        /// This function create patients.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">This handle the information necesary to make the insertion of the patient.</param>
        /// <returns>The model of the patient inserted.</returns>
        [GraphQLType(typeof(PersonType))]
        public async Task<Person> createPatient (
        [Service] hospitecContext db,
        [GraphQLNonNullType] CreatePersonInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create patient {1}", time, input.id));

            Console.WriteLine(s.ToString());

            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.id))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.firstName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a first name."));
            }

            if (string.IsNullOrEmpty(input.lastName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a last name."));
            }

            if (string.IsNullOrEmpty(input.phoneNumber))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid phone number"));
            }

            if (string.IsNullOrEmpty(input.canton))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a canton."));
            }

            if (string.IsNullOrEmpty(input.province))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a province."));
            }

            if (string.IsNullOrEmpty(input.address))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide an address."));
            }

            if (string.IsNullOrEmpty(input.id))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid birth date (Format: YYYY-MM-DD)."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            Person p = new Person
            {
                Identification = input.id,
                FirstName = input.firstName,
                LastName = input.lastName,
                PhoneNumber = input.phoneNumber,
                Canton = input.canton,
                Province = input.province,
                ExactAddress = input.address,
                BirthDate = input.birthDate
            };

            try
            {
                db.Person.Add(p);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A person with ID '{0}' already exists.", input.id));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));
                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch(Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This function adds a password to specfic patient.
        /// </summary>
        /// <param name="patientId">The identification code of the patient</param>
        /// <param name="password">The password of the patient.</param>
        /// <param name="db">Context of database</param>
        /// <returns>The model of the patient specified.</returns>
        [GraphQLType(typeof(PersonType))]
        public async Task<Person> addPassword(
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] string password,
            [Service] hospitecContext db)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Add password to patient {1}", time, patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid patient ID."));
            }

            if (string.IsNullOrEmpty(password))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a password."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            Patient p = new Patient
            {
                Identification = patientId,
                PatientPassword = password
            };

            try
            {
                db.Patient.Add(p);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A patient with ID '{0}' already exists.", patientId));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "There is no person with ID '{0}'", patientId));
                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return db.Person
                .Where(p => p.Identification.Equals(patientId))
                .FirstOrDefault();
        }

        /// <summary>
        /// This updates the patient personal info, except the password.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">This handles the patient indetifiying info and the fields to be updated.</param>
        /// <returns>The patient person info updated.</returns>
        //[Authorize(Policy = Constants.doctorRole)]
        //[Authorize(Policy = Constants.patientRole)]
        [GraphQLType(typeof(PersonType))]
        public async Task<Person> updatePatient(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdatePersonInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update patient {1}", time, input.oldId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.oldId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid patient ID."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE admin.person SET ");

            if (!string.IsNullOrEmpty(input.newId)) sql.AppendFormat("identification = '{0}',", input.newId);

            if (!string.IsNullOrEmpty(input.firstName)) sql.AppendFormat("first_name = '{0}',", input.firstName);

            if (!string.IsNullOrEmpty(input.lastName)) sql.AppendFormat("last_name = '{0}',", input.lastName);

            if (input.birthDate.HasValue) sql.AppendFormat("birth_date = '{0}',", input.birthDate);

            if (!string.IsNullOrEmpty(input.phoneNumber)) sql.AppendFormat("phone_number = '{0}',", input.phoneNumber);

            if (!string.IsNullOrEmpty(input.province)) sql.AppendFormat("province = '{0}',", input.province);

            if (!string.IsNullOrEmpty(input.canton)) sql.AppendFormat("canton = '{0}',", input.canton);

            if (!string.IsNullOrEmpty(input.address)) sql.AppendFormat("exact_address = '{0}',", input.address);

            sql.Replace(',', ' ', sql.Length - 1, 1); 

            sql.AppendFormat("WHERE identification = '{0}'", input.oldId);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "The ID '{0}' doesn't match with any person", input.oldId));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A patient with ID '{0}' already exists.", input.newId));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Person.FirstOrDefaultAsync(p => p.Identification.Equals(
                string.IsNullOrEmpty(input.newId) ? input.oldId : input.newId));
        }

        /// <summary>
        /// This deletes a patient and all his related info (clinica records, reservations, etc.)
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="id">The identification code of patient</param>
        /// <returns>The patient object with related info.</returns>
        //[Authorize(Policy = Constants.doctorRole)]
        //[Authorize(Policy = Constants.patientRole)]
        [GraphQLType(typeof(PersonType))]
        public async Task<Person> deletePatient(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string id)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete patient {1}", time, id));

            Console.WriteLine(s.ToString());



            if (string.IsNullOrEmpty(id))
            {
                throw new QueryException(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid patient ID."));
            }

            Person p = await db.Person.FirstOrDefaultAsync(p => p.Identification.Equals(id));

            if (p is null)
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "ID provided '{0}' doesn't match any patient.", id));

            try
            {
                await db.Database.ExecuteSqlRawAsync("CALL delete_patients_records_reservation({0})", p.Identification);

                db.Remove(p);

                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "You have a role attached, please delete the staff role associated in order to deleting your person personal data."));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This function deletes the personal info of specific staff member, this function must be executed after
        /// the function deleteStaff.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="id">The identification code of the staff member</param>
        /// <returns>An object with all info about the staff member.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(PersonType))]
        public async Task<Person> deleteStaffPerson(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string id)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete staff member {1}", time, id));

            Console.WriteLine(s.ToString());



            if (string.IsNullOrEmpty(id))
            {
                throw new QueryException(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid patient ID."));
            }

            Person p = await db.Person.FirstOrDefaultAsync(p => p.Identification.Equals(id));

            if (p is null)
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "ID provided '{0}' doesn't match any staff.", id));

            try
            {
                await db.Database.ExecuteSqlRawAsync("DELETE FROM admin.person WHERE identification = {0}", p.Identification);


            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "You have a patient attached, please delete the patient associated in order to deleting your personal data."));

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This function createe
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input"></param>
        /// <returns></returns>
        //[Authorize(Policy = Constants.doctorRole)]
        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> createRecord(
            [Service] hospitecContext db,
            [GraphQLNonNullType] CreateRecordInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create clinical record for patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.pathologyName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PATHOLOGY_NAME",
                    "You must provide a pathology name."));
            }

            if (!input.diagnosticDate.HasValue || input.diagnosticDate.Value == null)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_DIAGNOSTIC_DATE",
                    "You must provide a diagnostic date."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            ClinicRecord p = new ClinicRecord
            {
                Identification = input.patientId,
                DiagnosticDate = (DateTime)input.diagnosticDate,
                PathologyName = input.pathologyName,
                Treatment = input.treatment,
                MedicalProcedureRecord = new List<MedicalProcedureRecord>()
            };

            try
            {
                db.ClinicRecord.Add(p);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A clinic record with provided data already exists."));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "There is no patient with ID '{0}'", input.patientId));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This creates a procedure related with a specific clinic record entry of a patient
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">This handles the related identifiying information of patient
        /// and the procedure to insert.</param>
        /// <returns>The clinic record related</returns>
        //[Authorize(Policy = Constants.doctorRole)]
        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> createAppointment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] CreateAppointmentInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Attach procedure '{1}' to clinical record of patient {2}", time, input.procedureName, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.pathologyName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PATHOLOGY_NAME",
                    "You must provide a pathology name."));
            }

            if (!input.diagnosticDate.HasValue || input.diagnosticDate.Value == null)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_DIAGNOSTIC_DATE",
                    "You must provide a diagnostic date."));
            }

            if (!input.executionDate.HasValue || input.executionDate.Value == null)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_EXECUTION_DATE",
                    "You must provide an execution date."));
            }

            if (string.IsNullOrEmpty(input.procedureName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PROCEDURE_NAME",
                    "You must provide a procedure name."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            MedicalProcedureRecord p = new MedicalProcedureRecord
            {
                Identification = input.patientId,
                PathologyName = input.pathologyName,
                ProcedureName = input.procedureName,
                DiagnosticDate = (DateTime)input.diagnosticDate,
                OperationExecutionDate = (DateTime)input.executionDate
            };

            try
            {
                db.MedicalProcedureRecord.Add(p);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "This operation is already attached to record."));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            if (pgException.ConstraintName.Equals("medical_procedure_record_procedure_name_fkey"))
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "There is no procedure with that name"));
                            else
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "There is no clinic record with provided data."));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.ClinicRecord
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.PathologyName.Equals(input.pathologyName)
                            && p.DiagnosticDate.Equals(input.diagnosticDate))
                .Include(p => p.MedicalProcedureRecord)
                .ThenInclude(q => q.ProcedureNameNavigation)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This modifies a specific clinic record entry of a patient.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">This handles the identifiying information for the entry and the information to update.</param>
        /// <returns>The clinic record updated.</returns>
        //[Authorize(Policy = Constants.doctorRole)]
        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> updateRecord(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateRecordInput input)   
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update clinical record of patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.oldPathologyName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PATHOLOGY_NAME",
                    "You must provide a pathology name."));
            }

            if (!input.oldDiagnosticDate.HasValue || input.oldDiagnosticDate.Value == null)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_DIAGNOSTIC_DATE",
                    "You must provide a diagnostic date."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE doctor.clinic_record SET ");

            if (!string.IsNullOrEmpty(input.treatment)) sql.AppendFormat("treatment = '{0}',", input.treatment);

            if (!string.IsNullOrEmpty(input.newPathologyName)) sql.AppendFormat("pathology_name = '{0}',", input.newPathologyName);

            if (input.newDiagnosticDate.HasValue) sql.AppendFormat("diagnostic_date = '{0}',", input.newDiagnosticDate);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE identification = '{0}' ", input.patientId);

            sql.AppendFormat("AND pathology_name = '{0}' ", input.oldPathologyName);

            sql.AppendFormat("AND diagnostic_date = '{0}'", input.oldDiagnosticDate);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "The clinic record wasn't found"));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A clinic record with provided data already exists."));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.ClinicRecord
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.PathologyName.Equals(
                                string.IsNullOrEmpty(input.newPathologyName) ? input.oldPathologyName : input.newPathologyName)
                            && p.DiagnosticDate.Equals(
                                input.newDiagnosticDate.HasValue ? input.newDiagnosticDate : input.oldDiagnosticDate))
                .Include(p => p.MedicalProcedureRecord)
                .ThenInclude(q => q.ProcedureNameNavigation)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This deletes a specific clinic record entry.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="patientId">The patient identification code.</param>
        /// <param name="pathologyName">The pathology name</param>
        /// <param name="diagnosticDate">The pathology diagnostic date.</param>
        /// <returns>The clinic record deleted with the procedures related./returns>
        //[Authorize(Policy = Constants.doctorRole)]
        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> deleteRecord(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] string pathologyName,
            [GraphQLNonNullType] DateTime diagnosticDate)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete clinical record of patient {1}", time, patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(pathologyName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PATHOLOGY_NAME",
                    "You must provide a pathology name."));
            }

            if (diagnosticDate == null)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_DIAGNOSTIC_DATE",
                    "You must provide a diagnostic date."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            ClinicRecord p = await db.ClinicRecord
                .Include(p => p.MedicalProcedureRecord)
                    .ThenInclude(p => p.ProcedureNameNavigation)
                .FirstOrDefaultAsync(p => p.Identification.Equals(patientId)
                                          && p.DiagnosticDate.Equals(diagnosticDate)
                                          && p.PathologyName.Equals(pathologyName));

            if (p is null)
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "Data provided doesn't match any clinic record."));

            StringBuilder sql = new StringBuilder();

            sql.Append("DELETE FROM doctor.clinic_record WHERE ");

            sql.AppendLine(string.Format("identification = '{0}'", patientId));
            sql.AppendLine(string.Format("AND pathology_name = '{0}'", pathologyName));
            sql.AppendLine(string.Format("AND diagnostic_date = '{0}'", diagnosticDate));
            try
            {
                await db.Database.ExecuteSqlRawAsync(sql.ToString());
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This updates a specific medical procedure related with a clinic record entry.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information to locate the clinic record and the procedures to delete or to add.</param>
        /// <returns></returns>
        //[Authorize(Policy = Constants.doctorRole)]
        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> updateAppointment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateAppointmentInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update procedures in clinic record of patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.pathologyName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PATHOLOGY_NAME",
                    "You must provide a pathology name."));
            }

            if (!input.diagnosticDate.HasValue || input.diagnosticDate.Value == null)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_DIAGNOSTIC_DATE",
                    "You must provide a diagnostic date."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            if(!db.ClinicRecord.Any(p => p.Identification.Equals(input.patientId)
                                          && p.DiagnosticDate.Equals(input.diagnosticDate)
                                          && p.PathologyName.Equals(input.pathologyName)))
            {
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "There is no clinic record with provided data."));
            }

            try
            {
                foreach(ProcedureAppointmentInput p in input.deletedProcedures) {
                    MedicalProcedureRecord m = db.MedicalProcedureRecord
                        .Where(f => f.Identification.Equals(input.patientId)
                                    && f.PathologyName.Equals(input.pathologyName)
                                    && f.DiagnosticDate.Equals(input.diagnosticDate)
                                    && f.ProcedureName.Equals(p.procedureName)
                                    && f.OperationExecutionDate.Equals(p.executionDate))
                        .FirstOrDefault();

                    if (m is null)
                    {
                        continue;
                    }

                    db.MedicalProcedureRecord.Remove(m);
                }

                foreach (ProcedureAppointmentInput p in input.newProcedures)
                {
                    MedicalProcedureRecord m = new MedicalProcedureRecord
                    {
                        Identification = input.patientId,
                        PathologyName = input.pathologyName,
                        ProcedureName = p.procedureName,
                        DiagnosticDate = (DateTime)input.diagnosticDate,
                        OperationExecutionDate = p.executionDate
                    };

                    db.MedicalProcedureRecord.Add(m);
                }

                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some procedures are already attached to the clinic record."));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            if (pgException.ConstraintName.Equals("medical_procedure_record_procedure_name_fkey"))
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "There is no procedure with that name"));
                            else
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "There is no clinic record with provided data."));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.ClinicRecord
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.PathologyName.Equals(input.pathologyName)
                            && p.DiagnosticDate.Equals(input.diagnosticDate))
                .Include(p => p.MedicalProcedureRecord)
                .ThenInclude(q => q.ProcedureNameNavigation)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This validates the user inserted and generates a JWT Token to grant access to resources
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Auth> authentication(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string id,
            [GraphQLNonNullType] string password,
            string role)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Authentication of person {1}", time, id));

            Console.WriteLine(s.ToString());


            if (string.IsNullOrEmpty(id))
            {
                throw new QueryException(CustomErrorBuilder(
                    "VALUE_EMPTY",
                    "You must provide a user."));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new QueryException(CustomErrorBuilder(
                    "VALUE_EMPTY",
                    "You must provide a password."));
            }

            var dbpatient = await db.Patient.Where(s => s.Identification.Equals(id) && s.PatientPassword.Equals(password)).FirstOrDefaultAsync();

            if (role.Equals(Constants.patientRole))
            {
                var dbStaff = await db.Patient.Where(s => s.Identification.Equals(id) && s.PatientPassword.Equals(password)).FirstOrDefaultAsync();

                if (dbStaff is null)
                {
                    throw new QueryException(CustomErrorBuilder(
                        "UNAUTHORIZED",
                        "The user or password provided are not correct"));
                }

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, dbStaff.Identification),
                new Claim(Constants.RoleClaim, Constants.patientRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Constants.key));


                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token;

                token = new JwtSecurityToken(
                    issuer: Constants.Issuer,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(8),
                    signingCredentials: signingCredentials);


                var tokenR = new JwtSecurityTokenHandler().WriteToken(token);

                return new Auth
                {
                    accessKey = tokenR,
                    role = Constants.patientRole
                };
            }
            else
            {
                var dbStaff = await db.Staff.Where(s => s.Identification.Equals(id) && s.StaffPassword.Equals(password)).FirstOrDefaultAsync();

                if (dbStaff is null || !dbStaff.Name.Equals(role))
                {
                    throw new QueryException(CustomErrorBuilder(
                        "UNAUTHORIZED",
                        "The user or password provided are not correct"));
                }

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, dbStaff.Identification),
                new Claim(Constants.RoleClaim, dbStaff.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Constants.key));


                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token;

                token = new JwtSecurityToken(
                    issuer: Constants.Issuer,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(8),
                    signingCredentials: signingCredentials);


                var tokenR = new JwtSecurityTokenHandler().WriteToken(token);

                return new Auth
                {
                    accessKey = tokenR,
                    role = dbStaff.Name
                };
            }
        }

        /// <summary>
        /// This creates a new medical procedure
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="name">The name of new procedure</param>
        /// <param name="recoveringDays">The days that a patient requires to recover</param>
        /// <returns>The procedure object created.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ProcedureType))]
        public async Task<MedicalProcedures> createProcedure(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] short recoveringDays)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create medical procedure '{1}'", time, name));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(name))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid procedure name."));
            }

            if (errors.Count > 0) throw new QueryException(errors);


            MedicalProcedures m = new MedicalProcedures
            {
                Name = name,
                RecoveringDays = recoveringDays
            };

            try
            {
                db.MedicalProcedures.Add(m);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A procedure with provided data already exists."));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23514":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "You must insert valid days."));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This updates the information of specified procedure.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="oldName">The name of the medical procedure to update</param>
        /// <param name="newName">The new name of the medical procedure</param>
        /// <param name="recoveringDays">The new recovering days value, this and the new name can be ommited.</param>
        /// <returns>The procedure object updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ProcedureType))]
        public async Task<MedicalProcedures> updateProcedure(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string oldName,
            string newName,
            short? recoveringDays)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update medical procedure '{1}'", time, oldName));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(oldName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid procedure name."));
            }

            if (string.IsNullOrEmpty(newName) && !recoveringDays.HasValue)
            {
                errors.Add(CustomErrorBuilder(
                    "NOTHING_TO_CHANGE",
                    "There are no data to insert."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE doctor.medical_procedures SET ");

            if (!string.IsNullOrEmpty(newName)) sql.AppendFormat("name = '{0}',", newName);

            if (recoveringDays.HasValue) sql.AppendFormat("recovering_days = {0},", recoveringDays);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE name = '{0}' ", oldName);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                       "NOT_FOUND",
                       "There is no procedure with name '{0}'", oldName));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A procedure with provided data already exists."));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23514":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "You must insert valid days."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "There are some records with associated procedures, please delete them before changing this name."));

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch(QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.MedicalProcedures
                .FirstOrDefaultAsync(p => p.Name.Equals(string.IsNullOrEmpty(newName) ? oldName : newName));
        }

        /// <summary>
        /// This deletes the medical procedure specified.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="name">The name of the procedure to delete</param>
        /// <returns>The object of the medical procedure deleted.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ProcedureType))]
        public async Task<MedicalProcedures> deleteProcedure(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string name)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete medical procedure '{1}'", time, name));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(name))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid procedure name."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("DELETE FROM doctor.medical_procedures WHERE name = '{0}'", name);

            MedicalProcedures m = await db.MedicalProcedures
                .FirstOrDefaultAsync(p => p.Name.Equals(name));

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                       "NOT_FOUND",
                       "There is no procedure with name '{0}'", name));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "There are some records with associated procedures, please delete them before deleting this procedure."));

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This adds a new medical room
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">This handles the information to insert.</param>
        /// <returns>The medical room object created.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(MedicalRoomType))]
        public async Task<MedicalRoom> addMedicalRoom(
            [Service] hospitecContext db,
            [GraphQLNonNullType] CreateMedicalRoomInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create medical room {1}", time, input.id));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.name))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid name."));
            }

            if (string.IsNullOrEmpty(input.careType))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid care type name."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            MedicalRoom m = new MedicalRoom
            {
                Name = input.name,
                IdRoom = input.id,
                FloorNumber = input.floorNumber,
                Capacity = input.capacity,
                CareType = input.careType
            };

            try
            {
                db.MedicalRoom.Add(m);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A medical room with id '{0}' already exists.", input.id));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23514":

                            switch(pgException.ConstraintName){
                                case "medical_room_capacity_check":
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid capacity value."));

                                case "medical_room_id_room_check":
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid ID."));

                                default:
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid data."));
                            }

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This updates the medical room specified.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The identifiying information of the medical room to update and the fields to update.</param>
        /// <returns>The medical room updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(MedicalRoomType))]
        public async Task<MedicalRoom> updateMedicalRoom(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateMedicalRoomInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update medical room {1}", time, input.oldId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (input.oldId < 1)
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid room id."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE doctor.medical_room SET ");

            if (!string.IsNullOrEmpty(input.name)) sql.AppendFormat("name = '{0}',", input.name);

            if (!string.IsNullOrEmpty(input.careType)) sql.AppendFormat("care_type = '{0}',", input.careType);

            if (input.newId.HasValue) sql.AppendFormat("id_room = {0},", input.newId);

            if (input.floorNumber.HasValue) sql.AppendFormat("floor_number = {0},", input.floorNumber);

            if (input.capacity.HasValue) sql.AppendFormat("capacity = {0},", input.capacity);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE id_room = {0}", input.oldId);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                       "NOT_FOUND",
                       "There is no procedure with name '{0}'", input.oldId));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A room with id '{0}' data already exists.", input.newId));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23514":
                        switch (pgException.ConstraintName)
                        {
                            case "medical_room_capacity_check":
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid capacity value."));

                            case "medical_room_id_room_check":
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid ID."));

                            default:
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid data."));
                        }

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.MedicalRoom
                .FirstOrDefaultAsync(p => p.IdRoom.Equals(input.newId.HasValue ? input.newId : input.oldId));
        }

        /// <summary>
        /// This creates new beds.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="id">The id number for the new bed.</param>
        /// <param name="isICU">True if the bed is for intensive care unite or false if it's not.</param>
        /// <param name="idRoom">The medical room where the bed is, this is optional</param>
        /// <returns>The bed object created.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(BedType))]
        public async Task<Bed> createBed(
            [Service] hospitecContext db,
            [GraphQLNonNullType] int id,
            [GraphQLNonNullType] bool isICU,
            int? idRoom)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create Bed {1}", time, id));

            Console.WriteLine(s.ToString());


            Bed m;
            try
            {
                m = new Bed
                {
                    IdBed = id,
                    IdRoom = idRoom,
                    IsIcu = isICU
                };

                db.Bed.Add(m);
                await db.SaveChangesAsync();

                return m;
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A bed with id '{0}' already exists.", id));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "There is no room with id '{0}'.", idRoom));

                        case "23514":

                            switch (pgException.ConstraintName)
                            {
                                case "bed_id_bed_check":
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid id value."));

                                default:
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid data."));
                            }

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return null;
        }

        /// <summary>
        /// This updates the specified bed.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information related with the bed to update.</param>
        /// <returns>The bed object updated.</returns>
        [GraphQLType(typeof(BedType))]
        public async Task<Bed> updateBed(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateBedInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update bed {1}", time, input.oldBedId));

            Console.WriteLine(s.ToString());


            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE doctor.bed SET ");

            if (input.newBedId.HasValue) sql.AppendFormat("id_bed = {0},", input.newBedId);

            if (input.isIcu.HasValue) sql.AppendFormat("is_icu = {0},", input.isIcu);

            if (input.idRoom.HasValue) sql.AppendFormat("id_room = {0},", input.idRoom);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE id_bed = {0}", input.oldBedId);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                       "NOT_FOUND",
                       "There is no bed with id '{0}'", input.oldBedId));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A bed with id '{0}' data already exists.", input.newBedId));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "There is no room with id '{0}'.", input.idRoom));

                    case "23514":
                        switch (pgException.ConstraintName)
                        {
                            case "bed_id_bed_check":
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid id."));

                            default:
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid data."));
                        }

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Bed
                .FirstOrDefaultAsync(p => p.IdBed.Equals(input.newBedId.HasValue ? input.newBedId : input.oldBedId));
        }

        /// <summary>
        /// This deleted the specified bed.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="bedId">The id number of the bed to delete.</param>
        /// <returns>The bed object deleted.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(BedType))]
        public async Task<Bed> deleteBed(
            [Service] hospitecContext db,
            [GraphQLNonNullType] int bedId)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete bed {1}", time, bedId));

            Console.WriteLine(s.ToString());


            if (!db.Bed.Any(p => p.IdBed.Equals(bedId)))
            {
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "There is no bed with id '{0}'", bedId));
            }

            Bed m = await db.Bed
                .FirstOrDefaultAsync(p => p.IdBed.Equals(bedId));

            try
            {
                await db.Database.ExecuteSqlRawAsync("DELETE FROM doctor.bed WHERE id_bed = {0}", bedId);
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                               pgException,
                               "This bed has some reservations attached, please deleted them before deleting this bed"));

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This adds new medical equipment to specified bed.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="bedId">The id number of the bed.</param>
        /// <param name="equipmentSN">The serial number of the equipment tool selected.</param>
        /// <returns>The bed updated with the new equipment</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(BedType))]
        public async Task<Bed> addEquipment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] int bedId,
            [GraphQLNonNullType] string equipmentSN)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Attach equipment '{1}' to bed {2}", time, equipmentSN, bedId));

            Console.WriteLine(s.ToString());


            if (string.IsNullOrEmpty(equipmentSN))
            {
                throw new QueryException(CustomErrorBuilder(
                    "INVALID_PARAM",
                    "There is no equipment with serial number '{0}'", equipmentSN));
            }

            MedicalEquipmentBed m = new MedicalEquipmentBed
            {
                IdBed = bedId,
                SerialNumber = equipmentSN
            };

            try
            {
                db.MedicalEquipmentBed.Add(m);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A bed with this equipment already exists."));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "There is no bed '{0}' / procedure '{1}' with this id.", bedId, equipmentSN));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Bed.FirstOrDefaultAsync(p => p.IdBed.Equals(bedId));
        }

        /// <summary>
        /// This changes the equipment of specified bed.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="bedId">The id of the bed to change</param>
        /// <param name="newEquipmentSN">The serial numbers of all equipment to add.</param>
        /// <param name="deletedEquipmentSN">The serial numbers of all equipment to delete.</param>
        /// <returns>The bed updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(BedType))]
        public async Task<Bed> updateBedEquipment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] int bedId,
            ICollection<string> newEquipmentSN,
            ICollection<string> deletedEquipmentSN)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update equipment for bed {1}", time, bedId));

            Console.WriteLine(s.ToString());


            if (!db.Bed.Any(p => p.IdBed.Equals(bedId)))
            {
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "There is no bed with id '{0}'", bedId));
            }

            try
            {
                foreach (string p in deletedEquipmentSN)
                {
                    MedicalEquipmentBed m = db.MedicalEquipmentBed
                        .Where(f => f.IdBed.Equals(bedId)
                                    && f.SerialNumber.Equals(p))
                        .FirstOrDefault();

                    if (m is null)
                    {
                        continue;
                    }

                    db.MedicalEquipmentBed.Remove(m);
                }

                foreach (string p in newEquipmentSN)
                {
                    MedicalEquipmentBed m = new MedicalEquipmentBed
                    {
                        IdBed = bedId,
                        SerialNumber = p
                    };

                    db.MedicalEquipmentBed.Add(m);
                }

                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some tools required are already attached to bed with id '{0}'", bedId));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                   pgException,
                                   "There are some tools with wrong serial number"));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Bed
                .FirstOrDefaultAsync(p => p.IdBed.Equals(bedId));
        }

        /// <summary>
        /// This creates a new staff person entity.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The personal info for the new staff member person.</param>
        /// <returns>The staff object created.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(StaffType))]
        public async Task<Staff> createStaffPerson(
        [Service] hospitecContext db,
        [GraphQLNonNullType] CreatePersonInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create staff member person {1}", time, input.id));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.id))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.firstName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a first name."));
            }

            if (string.IsNullOrEmpty(input.lastName))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a last name."));
            }

            if (string.IsNullOrEmpty(input.phoneNumber))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid phone number"));
            }

            if (string.IsNullOrEmpty(input.canton))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a canton."));
            }

            if (string.IsNullOrEmpty(input.province))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a province."));
            }

            if (string.IsNullOrEmpty(input.address))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide an address."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            Person p = new Person
            {
                Identification = input.id,
                FirstName = input.firstName,
                LastName = input.lastName,
                PhoneNumber = input.phoneNumber,
                Canton = input.canton,
                Province = input.province,
                ExactAddress = input.address,
                BirthDate = input.birthDate
            };

            try
            {
                db.Person.Add(p);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A person with ID '{0}' already exists.", input.id));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));
                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return new Staff
            {
                Identification = input.id
            };
        }

        /// <summary>
        /// This create the staff member as it is.
        /// This has to be executed after createPersonStaff function
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information related with the staff member (admission date, password, etc.)</param>
        /// <returns>The staff created</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(StaffType))]
        public async Task<Staff> createStaff(
        [Service] hospitecContext db,
        [GraphQLNonNullType] CreateStaffInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create staff member info for person {1}", time, input.id));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.id))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.role))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid role name."));
            }

            if (string.IsNullOrEmpty(input.password))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid password."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            Staff p = new Staff
            {
                Identification = input.id,
                AdmissionDate = input.admissionDate,
                Name = input.role,
                StaffPassword = input.password
            };

            try
            {
                db.Staff.Add(p);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A staff with ID '{0}' and role '{1}' already exists.", input.id, input.role));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));
                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This updates the personal info of the specified staff member.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The identifiying information and fields to update of the staff member</param>
        /// <returns>The staff member updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(StaffType))]
        public async Task<Staff> updateStaffPerson(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdatePersonInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update staff personal info for member {1}", time, input.oldId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.oldId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid patient ID."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE admin.person SET ");

            if (!string.IsNullOrEmpty(input.newId)) sql.AppendFormat("identification = '{0}',", input.newId);

            if (!string.IsNullOrEmpty(input.firstName)) sql.AppendFormat("first_name = '{0}',", input.firstName);

            if (!string.IsNullOrEmpty(input.lastName)) sql.AppendFormat("last_name = '{0}',", input.lastName);

            if (input.birthDate.HasValue) sql.AppendFormat("birth_date = '{0}',", input.birthDate);

            if (!string.IsNullOrEmpty(input.phoneNumber)) sql.AppendFormat("phone_number = '{0}',", input.phoneNumber);

            if (!string.IsNullOrEmpty(input.province)) sql.AppendFormat("province = '{0}',", input.province);

            if (!string.IsNullOrEmpty(input.canton)) sql.AppendFormat("canton = '{0}',", input.canton);

            if (!string.IsNullOrEmpty(input.address)) sql.AppendFormat("exact_address = '{0}',", input.address);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE identification = '{0}'", input.oldId);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "The ID '{0}' doesn't match with any person", input.oldId));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A person with ID '{0}' already exists.", input.newId));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            var staff = await db.Staff
                .FirstOrDefaultAsync(p => p.Identification.Equals(
                    string.IsNullOrEmpty(input.newId) ? input.oldId : input.newId));

            if(staff is null)
            {
                return new Staff
                {
                    Identification = string.IsNullOrEmpty(input.newId) ? input.oldId : input.newId
                };
            } 
            else
            {
                return staff;
            }
        }

        /// <summary>
        /// This updates the information related with the staff member (admission date, role, etc.)
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">Identifiying information and fields to update.</param>
        /// <returns>The updated staff member object.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(StaffType))]
        public async Task<Staff> updateStaff(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateStaffInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update staff information of member {1}", time, input.id));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.oldRole))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid role name."));
            }

            if (string.IsNullOrEmpty(input.id))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid id."));
            }

            if(string.IsNullOrEmpty(input.newRole)
                && string.IsNullOrEmpty(input.password)
                && !input.admissionDate.HasValue)
            {
                errors.Add(CustomErrorBuilder(
                    "NOTHING_TO_CHANGE",
                    "There is no data to insert."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE admin.staff SET ");

            if (!string.IsNullOrEmpty(input.newRole)) sql.AppendFormat("name = '{0}',", input.newRole);

            if (!string.IsNullOrEmpty(input.password)) sql.AppendFormat("staff_password = '{0}',", input.password);

            if (input.admissionDate.HasValue) sql.AppendFormat("admission_date = '{0}',", input.admissionDate);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE identification = '{0}' ", input.id);

            sql.AppendFormat("AND name = '{0}'", input.oldRole);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                       "NOT_FOUND",
                       "There is no staff with id '{0}' and role '{1}'", input.id, input.oldRole));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A staff with role '{0}' and id '{1}' already exists.", input.newRole, input.id));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "There is no role with name '{0}'.", input.newRole));

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Staff
                .FirstOrDefaultAsync(p => p.Identification.Equals(input.id)
                                            && p.Name.Equals(string.IsNullOrEmpty(input.newRole) ? input.oldRole : input.newRole));
        }

        /// <summary>
        /// This deletes the staff member with all his personal information.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="id">The identification number of the staff member.</param>
        /// <param name="role">The role of the member.</param>
        /// <returns>The staff object with all information.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(StaffType))]
        public async Task<Staff> deleteStaff(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string id,
            [GraphQLNonNullType] string role)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete staff member {1}", time, id));

            Console.WriteLine(s.ToString());


            if (string.IsNullOrEmpty(id))
            {
                throw new QueryException(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a valid patient ID."));
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new QueryException(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a role."));
            }

            Staff p = await db.Staff
                .FirstOrDefaultAsync(p => p.Identification.Equals(id)
                                            && p.Name.Equals(role));

            if (p is null)
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "ID '{0}' and role '{1}' provided doesn't match any staff.", id, role));

            try
            {
                db.Remove(p);

                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));
                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return p;
        }

        /// <summary>
        /// This creates new medical equipment.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information to create the equipment.</param>
        /// <returns>The new medical equipment object.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(EquipmentType))]
        public async Task<MedicalEquipment> createEquipment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] CreateEquipmentInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create medical equipment '{1}'", time, input.serialNumber));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.serialNumber))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a serial number"));
            }

            if (string.IsNullOrEmpty(input.name))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a name."));
            }

            if (string.IsNullOrEmpty(input.provider))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a provider."));
            }

            if (errors.Count > 0) throw new QueryException(errors);


            MedicalEquipment m = new MedicalEquipment
            {
                Name = input.name,
                SerialNumber = input.serialNumber,
                Provider = input.provider,
                Stock = input.stock
            };
            try
            {
                db.MedicalEquipment.Add(m);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "A equipment with id '{0}' already exists.", input.serialNumber));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23514":

                            switch (pgException.ConstraintName)
                            {
                                case "medical_equipment_stock_check":
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid stock value."));

                                default:
                                    throw new QueryException(CustomErrorBuilder(
                                        pgException,
                                        "You must insert valid data."));
                            }

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This updates the information of a specified equipment.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information to identify the equipment to update and the fields to update.</param>
        /// <returns>The equipment object updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(EquipmentType))]
        public async Task<MedicalEquipment> updateEquipment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateEquipmentInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update medical equipment '{1}'", time, input.oldSerialNumber));

            Console.WriteLine(s.ToString());


            if (string.IsNullOrEmpty(input.oldSerialNumber))
            {
                throw new QueryException(CustomErrorBuilder(
                    "INVALID_INPUT",
                    "You must provide a serial number"));
            }

            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE doctor.medical_equipment SET ");

            if (!string.IsNullOrEmpty(input.newSerialNumber)) sql.AppendFormat("serial_number = '{0}',", input.newSerialNumber);

            if (!string.IsNullOrEmpty(input.name)) sql.AppendFormat("name = '{0}',", input.name);

            if (!string.IsNullOrEmpty(input.provider)) sql.AppendFormat("provider = '{0}',", input.provider);

            if (input.stock.HasValue) sql.AppendFormat("stock = '{0}',", input.stock);

            sql.Replace(',', ' ', sql.Length - 1, 1);

            sql.AppendFormat("WHERE serial_number = '{0}'", input.oldSerialNumber);

            try
            {
                int rows = await db.Database.ExecuteSqlRawAsync(sql.ToString());

                if (rows < 1)
                {
                    throw new QueryException(CustomErrorBuilder(
                       "NOT_FOUND",
                       "There is no equipment with id '{0}'", input.oldSerialNumber));
                }
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "A equipment with id '{0}' already exists.", input.newSerialNumber));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23514":
                        switch (pgException.ConstraintName)
                        {
                            case "medical_equipment_stock_check":
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid stock value."));

                            default:
                                throw new QueryException(CustomErrorBuilder(
                                    pgException,
                                    "You must insert valid data."));
                        }

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.MedicalEquipment
                .FirstOrDefaultAsync(p => p.SerialNumber.Equals(string.IsNullOrEmpty(input.newSerialNumber) ? input.oldSerialNumber : input.newSerialNumber));
        }

        /// <summary>
        /// This deletes the specified medical room
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="id">The id number of the specified medical room.</param>
        /// <returns>The medical room deleted.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(MedicalRoomType))]
        public async Task<MedicalRoom> deleteMedicalRoom(
            [Service] hospitecContext db,
            [GraphQLNonNullType] int id)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete medical room {1}", time, id));

            Console.WriteLine(s.ToString());


            MedicalRoom m = await db.MedicalRoom
                .FirstOrDefaultAsync(p => p.IdRoom.Equals(id));

            if(m is null)
            {
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "There is no room with id '{0}'", id));
            }

            try
            {
                await db.Database.ExecuteSqlRawAsync("CALL delete_medical_room({0})", id);
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This deletes the medical equipment specified.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="serialNumber">The serial number of the equipment specified.</param>
        /// <returns>The equipment object deleted.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(EquipmentType))]
        public MedicalEquipment deleteEquipment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string serialNumber)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete medical equipment {1}", time, serialNumber));

            Console.WriteLine(s.ToString());


            MedicalEquipment m = db.MedicalEquipment
                .FirstOrDefault(p => p.SerialNumber.Equals(serialNumber));

            if (m is null)
            {
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "There is no equipment with S/N '{0}'", serialNumber));
            }

            try
            {
                int rows = db.Database.ExecuteSqlRaw("DELETE FROM doctor.medical_equipment WHERE serial_number = {0}", serialNumber);
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }

        /// <summary>
        /// This create a reservation for a specified patient and assigns a bed automatically
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information related with the reservation.</param>
        /// <returns></returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ReservationType))]
        public async Task<Reservation> createReservation(
            [Service] hospitecContext db,
            [GraphQLNonNullType] AddReservationInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create reservation for patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (db.Reservation
                .Any(p => p.Identification.Equals(input.patientId)
                          && (p.CheckOutDate.Value.CompareTo(input.checkInDate) > 0 
                             || p.CheckOutDate.Value.CompareTo(input.checkInDate) == 0)))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_CHECK_IN_DATE",
                    "You have a reservation active in this date, " +
                    "please insert a date after the check out date in the active reservation"));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            Reservation p = new Reservation
            {
                Identification = input.patientId,
                CheckInDate = input.checkInDate
            };

            try
            {
                db.Reservation.Add(p);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException() is PostgresException pgException)
                {
                    switch (pgException.SqlState)
                    {
                        case "23505":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "This reservation already exists."));

                        case "22001":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Some of values inserted are too long."));

                        case "23503":
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "The patient doesn't exists."));

                        default:
                            throw new QueryException(CustomErrorBuilder(
                                pgException,
                                "Unknown error"));
                    }
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Reservation
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.CheckInDate.Equals(input.checkInDate))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This creates procedures for a specified reservation
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information related with the reservation to update and the procedure to attach.</param>
        /// <returns>The reservation object updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ReservationType))]
        public async Task<Reservation> createProcedureReserved(
            [Service] hospitecContext db,
            [GraphQLNonNullType] AddProcedureReservedInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Attach procedure to reservation of patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(input.name))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_PROCEDURE_NAME",
                    "You must provide a valid procedure name."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            try
            {
                await db.Database
                    .ExecuteSqlRawAsync("CALL create_procedure_for_reservation({0}, {1}::Date, {2}, {3});",
                    input.patientId,
                    input.checkInDate.ToString("yyyy-MM-dd"),
                    input.icu, 
                    input.name);
            }
            catch (PostgresException pgException)
            {
                db.Database.ExecuteSqlRaw(
                    "DELETE FROM admin.reservation " +
                    "WHERE identification = {0} " +
                    "AND check_in_date = {1}", input.patientId, input.checkInDate);

                switch (pgException.SqlState)
                {
                    case "23505":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "This procedure is already attached to reservation."));

                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));

                    case "23503":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "The procedure specified doesn't exists."));

                    case "P0001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            pgException.Message));
                        break;

                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return await db.Reservation
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.CheckInDate.Equals(input.checkInDate))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This updates the reservation procedures attached.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information of the reservation to update and the procedures to add or delete.</param>
        /// <returns>The reservation object updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ReservationType))]
        public async Task<Reservation> updateReservationProcedures(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateReservationProceduresInput input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update procedures for reservation of patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            if (input.deletedProcedures != null)
            {
                foreach (string j in input.deletedProcedures)
                {
                    try
                    {
                        await db.Database
                            .ExecuteSqlRawAsync("CALL delete_procedure_for_reservation({0}, {1}::Date, {2});",
                            input.patientId,
                            input.checkInDate.ToString("yyyy-MM-dd"),
                            j);
                    }
                    catch (PostgresException pgException)
                    {
                        switch (pgException.SqlState)
                        {
                            case "22001":

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "Some of values inserted are too long."));
                                break;

                            case "P0001":

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "The procedure '{0}' doesn't exists.", j));
                                break;

                            case "23505":

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "The procedure '{0}' is already attached to reservation.", j));
                                break;

                            default:

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "Unknown error"));
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add(CustomErrorBuilder(
                                    e.GetType().ToString(),
                                    e.Message));
                    }
                }
            }

            if (input.newProcedures != null)
            {
                foreach (string j in input.newProcedures)
                {
                    try
                    {
                        await db.Database
                            .ExecuteSqlRawAsync("CALL create_procedure_for_reservation({0}, {1}::Date, {2}, {3});",
                            input.patientId,
                            input.checkInDate.ToString("yyyy-MM-dd"),
                            input.icu,
                            j);
                    }
                    catch (PostgresException pgException)
                    {
                        switch (pgException.SqlState)
                        {

                            case "22001":

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "Some of values inserted are too long."));
                                break;

                            case "P0001":

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "The procedure '{0}' doesn't exists.", j));
                                break;

                            default:

                                errors.Add(CustomErrorBuilder(
                                    pgException,
                                    "Unknown error"));
                                break;
                        }
                    }
                    catch (Exception e)
                    {

                        errors.Add(CustomErrorBuilder(
                                    e.GetType().ToString(),
                                    e.Message));
                    }
                }
            }

            if (errors.Count > 0) throw new QueryException(errors);

            return await db.Reservation
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.CheckInDate.Equals(input.checkInDate))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This udates the check in date of the reservation specified.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="input">The information of the reservation and the new check in date.</param>
        /// <returns>The reservation object updated.</returns>
        //[Authorize(Policy = Constants.adminRole)]
        [GraphQLType(typeof(ReservationType))]
        public async Task<Reservation> updateReservationCheckInDate(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateReservationCheckInDate input)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (PUT)", time));
            s.AppendLine(string.Format("{0}: Operation = Update check in date of reservation for patient {1}", time, input.patientId));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(input.patientId))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            try
            {
                await db.Database
                    .ExecuteSqlRawAsync("CALL update_reserved_check_in_date({0}, {1}::Date, {2}::Date, {3});",
                    input.patientId,
                    input.oldCheckInDate.ToString("yyyy-MM-dd"),
                    input.newCheckInDate,
                    input.icu);
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":

                        errors.Add(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                        break;

                    case "23505":

                        errors.Add(CustomErrorBuilder(
                            pgException,
                            "There is another reservation with this check in date"));
                        break;

                    case "P0001":

                        errors.Add(CustomErrorBuilder(
                            pgException,
                            pgException.Message));
                        break;

                    default:

                        errors.Add(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                        break;
                }
            }
            catch (Exception e)
            {
                errors.Add(CustomErrorBuilder(
                            e.GetType().ToString(),
                            e.Message));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            return await db.Reservation
                .Where(p => p.Identification.Equals(input.patientId)
                            && p.CheckInDate.Equals(input.newCheckInDate))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// This creates a new hospital evaluation entry.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="patientID">The patient identification code who is evaluating</param>
        /// <param name="category">The category to evaluate.</param>
        /// <param name="evaluation">The value of the evaluation in scale of 1 to 5.</param>
        /// <param name="date">The date of the evaluation.</param>
        /// <returns>A boolean to indicate if the evaluation was inserted or not.</returns>
        //[Authorize(Policy = Constants.patientRole)]
        public bool HospitalEvaluation(
            [Service] MongoDatabase db,
            [GraphQLNonNullType] string patientID,
            [GraphQLNonNullType] int category,
            [GraphQLNonNullType] int evaluation,
            [GraphQLNonNullType] DateTime date)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create hospital evaluation", time));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(patientID))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            return db.generateHospitalEvaluation(patientID, category, evaluation, date);
        }

        /// <summary>
        /// This creates a evaluation of staff member specified.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="patientID">The patient who is making the evaluation.</param>
        /// <param name="staffID">The staff member identification code who is under evaluation.</param>
        /// <param name="evaluation">The evaluation value in scale of 1 to 5</param>
        /// <param name="date">The date of the evaluation.</param>
        /// <returns>A boolean to indicate if the evaluation was inserted or not.</returns>
        /// //[Authorize(Policy = Constants.patientRole)]
        public bool staffEvaluation(
            [Service] MongoDatabase db,
            [GraphQLNonNullType] string patientID,
            [GraphQLNonNullType] string staffID,
            [GraphQLNonNullType] int evaluation,
            [GraphQLNonNullType] DateTime date)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (POST)", time));
            s.AppendLine(string.Format("{0}: Operation = Create staff evaluation", time));

            Console.WriteLine(s.ToString());


            List<IError> errors = new List<IError>();

            if (string.IsNullOrEmpty(patientID))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (string.IsNullOrEmpty(staffID))
            {
                errors.Add(CustomErrorBuilder(
                    "INVALID_ID",
                    "You must provide a valid ID."));
            }

            if (errors.Count > 0) throw new QueryException(errors);

            return db.generateStaffEvaluation(patientID, staffID, evaluation, date);
        }

        /// <summary>
        /// This method deletes a reservation and it's procedures related.
        /// </summary>
        /// <param name="db">Context of database</param>
        /// <param name="patientId">The identification code of the patient</param>
        /// <param name="checkInDate">The </param>
        /// <returns></returns>
        //[Authorize(Policy = Constants.patientRole)]
        [GraphQLType(typeof(ReservationType))]
        public Reservation deleteReservation(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] DateTime checkInDate)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Mutation (DELETE)", time));
            s.AppendLine(string.Format("{0}: Operation = Delete reservation of patient {1}", time, patientId));

            Console.WriteLine(s.ToString());


            Reservation m = db.Reservation
                .FirstOrDefault(p => p.Identification.Equals(patientId)
                                        && p.CheckInDate.Equals(checkInDate));

            if (m is null)
            {
                throw new QueryException(CustomErrorBuilder(
                    "NOT_FOUND",
                    "There is no reservation of patient '{0}' and check in date '{1}'", patientId, checkInDate.ToString("yyyy-MM-dd")));
            }

            try
            {
                int rows = db.Database.ExecuteSqlRaw(
                    "DELETE FROM admin.reservation " +
                    "WHERE identification = {0} " +
                    "AND check_in_date = {1}", patientId, checkInDate);
            }
            catch (PostgresException pgException)
            {
                switch (pgException.SqlState)
                {
                    case "22001":
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Some of values inserted are too long."));
                    default:
                        throw new QueryException(CustomErrorBuilder(
                            pgException,
                            "Unknown error"));
                }
            }
            catch (Exception e)
            {
                throw new QueryException(CustomErrorBuilder(
                                e.GetType().ToString(),
                                e.Message));
            }

            return m;
        }
    }
}