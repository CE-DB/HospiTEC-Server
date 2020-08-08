using HospiTec_Server.database.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// This maps the fields of the graphql type for the medical procedure entity
    /// </summary>
    public class ProcedureType : ObjectType<MedicalProcedures>
    {
        protected override void Configure(IObjectTypeDescriptor<MedicalProcedures> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("Procedure");

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.RecoveringDays)
                .Type<NonNullType<ShortType>>();
        }
    }
}
