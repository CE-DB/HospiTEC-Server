using HospiTec_Server.DBModels;
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

namespace HospiTec_Server.Logic.Graphql
{
    public class Mutation
    {
        /// <summary>
        /// This is for make an error to throw using a list, in this way you can throw many errors at once.
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

        [GraphQLType(typeof(PersonType))]
        public async Task<Person> createPatient (
        [Service] hospitecContext db,
        [GraphQLNonNullType] CreatePersonInput input)
        {
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

        [GraphQLType(typeof(PersonType))]
        public async Task<Person> addPassword(
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] string password,
            [Service] hospitecContext db)
        {
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

        [GraphQLType(typeof(PersonType))]
        public async Task<Person> updatePatient(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdatePersonInput input)
        {
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

        [GraphQLType(typeof(PersonType))]
        public async Task<Person> deletePatient(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string id)
        {

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


        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> createRecord(
            [Service] hospitecContext db,
            [GraphQLNonNullType] CreateRecordInput input)
        {
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

        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> createAppointment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] CreateAppointmentInput input)
        {
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


        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> updateRecord(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateRecordInput input)
        {
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

        [GraphQLType(typeof(RecordType))]
        public async Task<ClinicRecord> updateAppointment(
            [Service] hospitecContext db,
            [GraphQLNonNullType] UpdateAppointmentInput input)
        {
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
    }
}
