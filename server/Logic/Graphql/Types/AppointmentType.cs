using HospiTec_Server.database.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// This maps the fields of procedure related with specific clinic record.
    /// </summary>
    public class AppointmentType : ObjectType<MedicalProcedureRecord>
    {
        protected override void Configure(IObjectTypeDescriptor<MedicalProcedureRecord> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("Appointment");

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.ProcedureNameNavigation)
                .Name("procedure")
                .Description("This is the procedure object related")
                .Type<NonNullType<ProcedureType>>();

            descriptor.Field(e => e.OperationExecutionDate)
                .Name("executionDate")
                .Description("Execution date of procedure.")
                .Type<NonNullType<DateType>>();
        }
    }
}
