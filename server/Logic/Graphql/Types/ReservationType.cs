using HospiTec_Server.database.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

            descriptor.Field(e => e.IdBed)
                .Type<IntType>();

            descriptor.Field("procedures")
                .Type<ListType<ProcedureType>>()
                .Resolver(ctx => {

                    return ctx.Service<hospitecContext>()
                        .MedicalProcedureReservation
                        .Where(e => e.Identification.Equals(ctx.Parent<Reservation>().Identification)
                                    && e.CheckInDate.Equals(ctx.Parent<Reservation>().CheckInDate))
                        .Include(e => e.NameNavigation)
                        .Select(e => e.NameNavigation)
                        .ToListAsync();

                });
        }       
    }
}
