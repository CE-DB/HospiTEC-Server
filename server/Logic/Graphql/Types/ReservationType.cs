using HospiTec_Server.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class ReservationType : ObjectType<Reservation>
    {
        protected override void Configure(IObjectTypeDescriptor<Reservation> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.Identification)
                .Name("patientId")
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.CheckInDate)
                .Type<NonNullType<DateType>>();

            descriptor.Field(e => e.CheckOutDate)
                .Type<DateType>();

            descriptor.Field("bedId")
                .Type<IntType>()
                .Resolver(ctx => {

                    return ctx.Service<hospitecContext>()
                        .ReservationBed
                        .Where(e => e.Identification.Equals(ctx.Parent<Reservation>().Identification)
                                    && e.CheckInDate.Equals(ctx.Parent<Reservation>().CheckInDate))
                        .Select(e => e.IdBed)
                        .FirstOrDefault();

                });

            descriptor.Field("procedures")
                .Type<ListType<ProcedureType>>()
                .Resolver(ctx => {

                    return ctx.Service<hospitecContext>()
                        .MedicalProcedureReservation
                        .Where(e => e.Identification.Equals(ctx.Parent<Reservation>().Identification)
                                    && e.CheckInDate.Equals(ctx.Parent<Reservation>().CheckInDate))
                        .Include(e => e.NameNavigation)
                        .Select(e => e.NameNavigation)
                        .FirstOrDefault();

                });
        }       
    }
}
