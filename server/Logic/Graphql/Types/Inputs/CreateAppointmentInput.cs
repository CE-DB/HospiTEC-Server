using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class CreateAppointmentInput
    {
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public string pathologyName { get; set; }
        [GraphQLNonNullType]
        public string procedureName { get; set; }
        [GraphQLNonNullType]
        public DateTime diagnosticDate { get; set; }

    }
}
