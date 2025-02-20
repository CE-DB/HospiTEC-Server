﻿using HospiTec_Server.database.DBModels;
using HotChocolate.Types;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// This maps the fields of the graphql type for the clinical record entity
    /// </summary>
    public class RecordType : ObjectType<ClinicRecord>
    {
        protected override void Configure(IObjectTypeDescriptor<ClinicRecord> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("Record");

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
