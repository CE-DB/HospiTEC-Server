using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class AddProcedureReservedInput
    {
        [GraphQLNonNullType]
        public string name { get; set; }
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public DateTime checkInDate { get; set; }
    }
}
