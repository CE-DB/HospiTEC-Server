using HospiTec_Server.CotecModels;
using HospiTec_Server.database.DBModels;
using HospiTec_Server.Logic.Graphql.Types;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql
{
    /// <summary>
    /// This class is for manage the query operations in graphql
    /// </summary>
    public class Query
    {
        /// <summary>
        /// This one gets all the patients available in database.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <param name="cotec">Service of database context for CoTEC-2020 database</param>
        /// <returns>Return the list of patients, the model used was the person because all info of patients is only
        /// th personal info, other things are attached using the graphql type.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<PersonType>>>))]
        public async Task<List<Person>> patients(
            [Service] hospitecContext db,
            [Service] CoTEC_DBContext cotec)
        {
            List<Person> local = await db.Person.ToListAsync();

            List<CotecModels.Patient> cotecremote = await cotec.Patient
                .Where(p => p.Country.Equals("Costa Rica, Republic of"))
                .ToListAsync();

            foreach (CotecModels.Patient p in cotecremote)
            {
                local.Add(new Person {
                    Identification = p.Identification,
                    BirthDate = null,
                    Canton = null,
                    ExactAddress = null,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PhoneNumber = null,
                    Province = p.Region,
                    External = true
                });
            }

            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = patients", time));

            Console.WriteLine(s.ToString());

            return local;
        }

        /// <summary>
        /// This function gets all reservations for one specific patient.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <param name="patientId">The identification code of patient.</param>
        /// <returns>A list with all the reservations related with the patient selected.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ReservationType>>>))]
        public async Task<List<Reservation>> reservations(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {

            try
            {
                StringBuilder s = new StringBuilder();

                string time = (DateTime.UtcNow
                                            .AddHours(-6))
                                            .ToString("yyyy/MM/dd - hh:mm:ss");

                s.AppendLine(string
                    .Format("{0}: Type      = Query (GET)", time));
                s.AppendLine(string.Format("{0}: Operation = Reservations for patient {1}", time, patientId));

                Console.WriteLine(s.ToString());

                return await db.Reservation
                .Where(p => p.Identification.Equals(patientId))
                .ToListAsync();
            }
            catch(Exception e)
            {
                throw new QueryException(ErrorBuilder.New()
                           .SetMessage(e.Message)
                           .SetCode(e.GetType().FullName)
                           .Build());
            }
        }

        /// <summary>
        /// This function gets all the medical equipment available in the database.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <returns>A list with the medical equipment models.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<EquipmentType>>>))]
        public async Task<List<MedicalEquipment>> equipment(
            [Service] hospitecContext db)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = equipment", time));

            Console.WriteLine(s.ToString());

            return await db.MedicalEquipment
                .ToListAsync();
        }

        /// <summary>
        /// This functions gets all the clinical records of one specific patient.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <param name="patientId">Identification code of patient.</param>
        /// <returns>A list with all clinic records.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<RecordType>>>))]
        public async Task<List<ClinicRecord>> records(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = Clinical records for patient {1}", time, patientId));

            Console.WriteLine(s.ToString());

            return await db.ClinicRecord
                .Where(p => p.Identification.Equals(patientId))
                .Include(p => p.MedicalProcedureRecord)
                    .ThenInclude(p => p.ProcedureNameNavigation)
                .ToListAsync();
        }

        /// <summary>
        /// This function gets all medical rooms available.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <returns>A list with all medical rooms.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<MedicalRoomType>>>))]
        public async Task<List<MedicalRoom>> medicalRooms(
            [Service] hospitecContext db)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = Medical rooms", time));

            Console.WriteLine(s.ToString());

            return await db.MedicalRoom.ToListAsync();
        }

        /// <summary>
        /// This functions gets all beds available.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <returns>A list with all beds.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<BedType>>>))]
        public async Task<List<Bed>> beds(
            [Service] hospitecContext db)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = Beds", time));

            Console.WriteLine(s.ToString());

            return await db.Bed.ToListAsync();
        }

        /// <summary>
        /// This function gets all medical procedures available.
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <returns>A list with all medical procedures.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ProcedureType>>>))]
        public async Task<List<MedicalProcedures>> procedures(
            [Service] hospitecContext db)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = Medical procedures", time));

            Console.WriteLine(s.ToString());

            return await db.MedicalProcedures.ToListAsync();
        }

        /// <summary>
        /// This functions gets all staff members available
        /// </summary>
        /// <param name="db">Service of the database context</param>
        /// <returns>A list with all informations of the staff, this includes the personal info related in a model of person.</returns>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<StaffType>>>))]
        public async Task<List<Staff>> staff(
            [Service] hospitecContext db)
        {
            StringBuilder s = new StringBuilder();

            string time = (DateTime.UtcNow
                                        .AddHours(-6))
                                        .ToString("yyyy/MM/dd - hh:mm:ss");

            s.AppendLine(string
                .Format("{0}: Type      = Query (GET)", time));
            s.AppendLine(string.Format("{0}: Operation = Staff", time));

            Console.WriteLine(s.ToString());

            return await db.Staff.ToListAsync();
        }
    }
}
