﻿using HospiTec_Server.DBModels;
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
            [Service] hospitecContext db)
        {
            return await db.Person.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ReservationType>>>))]
        public async Task<List<Reservation>> reservations(
            [Service] hospitecContext db)
        {
            return await db.Reservation.ToListAsync();
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
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            return await db.MedicalRoom.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<BedType>>>))]
        public async Task<List<Bed>> beds(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            return await db.Bed.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ProcedureType>>>))]
        public async Task<List<MedicalProcedures>> procedures(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            return await db.MedicalProcedures.ToListAsync();
        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<StaffType>>>))]
        public async Task<List<Staff>> staff(
            [Service] hospitecContext db,
            [GraphQLNonNullType] string patientId)
        {
            return await db.Staff.ToListAsync();
        }
    }
}
