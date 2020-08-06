using HospiTec_Server.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
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
