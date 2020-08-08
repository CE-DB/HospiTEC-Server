using HospiTec_Server.CotecModels;
using HospiTec_Server.database.DBModels;
using HospiTec_Server.Logic.Graphql.Types;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql
{
    public class Query
    {
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

            return local;
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ReservationType>>>))]
        public async Task<List<Reservation>> reservations(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            return await db.Reservation
                .Where(p => p.Identification.Equals(patientId))
                .ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<EquipmentType>>>))]
        public async Task<List<MedicalEquipment>> equipment(
            [Service] hospitecContext db)
        {
            return await db.MedicalEquipment
                .ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<RecordType>>>))]
        public async Task<List<ClinicRecord>> records(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            return await db.ClinicRecord
                .Where(p => p.Identification.Equals(patientId))
                .Include(p => p.MedicalProcedureRecord)
                    .ThenInclude(p => p.ProcedureNameNavigation)
                .ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<MedicalRoomType>>>))]
        public async Task<List<MedicalRoom>> medicalRooms(
            [Service] hospitecContext db)
        {
            return await db.MedicalRoom.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<BedType>>>))]
        public async Task<List<Bed>> beds(
            [Service] hospitecContext db)
        {
            return await db.Bed.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ProcedureType>>>))]
        public async Task<List<MedicalProcedures>> procedures(
            [Service] hospitecContext db)
        {
            return await db.MedicalProcedures.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<StaffType>>>))]
        public async Task<List<Staff>> staff(
            [Service] hospitecContext db)
        {
            return await db.Staff.ToListAsync();
        }
    }
}
