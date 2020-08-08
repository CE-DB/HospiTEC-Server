using HospiTec_Server.database.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// Maps the fields of the graphql type for the medical equipment entity
    /// </summary>
    public class EquipmentType : ObjectType<MedicalEquipment>
    {
        protected override void Configure(IObjectTypeDescriptor<MedicalEquipment> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.SerialNumber)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.Stock)
                .Type<NonNullType<IntType>>();

            descriptor.Field(e => e.Provider)
                .Type<NonNullType<StringType>>();
        }
    }
}
