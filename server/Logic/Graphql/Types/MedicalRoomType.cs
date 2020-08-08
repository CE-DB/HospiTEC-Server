using HospiTec_Server.database.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// This maps the fields of the graphql type for the medical room entity
    /// </summary>
    public class MedicalRoomType : ObjectType<MedicalRoom>
    {
        protected override void Configure(IObjectTypeDescriptor<MedicalRoom> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.IdRoom)
                .Name("id")
                .Type<NonNullType<IntType>>();

            descriptor.Field(e => e.FloorNumber)
                .Type<NonNullType<ShortType>>();

            descriptor.Field(e => e.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.Capacity)
                .Type<NonNullType<ShortType>>();

            descriptor.Field(e => e.CareType)
                .Type<NonNullType<StringType>>();
        }
    }
}
