using HospiTec_Server.DBModels;
using HotChocolate.Types;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class RecordType : ObjectType<ClinicRecord>
    {
        protected override void Configure(IObjectTypeDescriptor<ClinicRecord> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(e => e.PathologyName)
                .Type<NonNullType<StringType>>();

            descriptor.Field(e => e.DiagnosticDate)
                .Type<NonNullType<DateType>>();

            descriptor.Field(e => e.Treatment)
                .Type<StringType>();

            descriptor.Field(e => e.MedicalProcedureRecord)
                .Name("Appointment")
                .Type<ListType<NonNullType<AppointmentType>>>();
        }
    }
}
