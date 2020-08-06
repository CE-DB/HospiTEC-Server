using HospiTec_Server.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class BedType : ObjectType<Bed>
    {
        protected override void Configure(IObjectTypeDescriptor<Bed> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.IdBed)
                .Name("id")
                .Type<NonNullType<IntType>>();

            descriptor.Field(e => e.IsIcu)
                .Type<NonNullType<BooleanType>>();

            descriptor.Field(e => e.IdRoom)
                .Type<IntType>();

            descriptor.Field("equipment")
                .Type<ListType<NonNullType<EquipmentType>>>()
                .Resolver(ctx => {

                    return ctx.Service<hospitecContext>()
                        .MedicalEquipmentBed
                        .Where(e => e.IdBed.Equals(ctx.Parent<Bed>().IdBed))
                        .Include(e => e.SerialNumberNavigation)
                        .Select(e => e.SerialNumberNavigation)
                        .ToList();
                });
        }
    }
}
