using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class ProcedureAppointmentInput
    {
        [GraphQLNonNullType]
        public string procedureName { get; set; }
        [GraphQLNonNullType]
        public DateTime executionDate { get; set; }
    }
}
