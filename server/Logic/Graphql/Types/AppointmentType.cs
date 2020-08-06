using HospiTec_Server.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class AppointmentType : ObjectType<MedicalProcedureRecord>
    {
        protected override void Configure(IObjectTypeDescriptor<MedicalProcedureRecord> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("Appointment");

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.ProcedureNameNavigation)
                .Name("procedure")
                .Type<NonNullType<ProcedureType>>();

            descriptor.Field(e => e.OperationExecutionDate)
                .Name("executionDate")
                .Type<NonNullType<DateType>>();
        }
    }
}
