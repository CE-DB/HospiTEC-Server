using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class AddBedReservedInput
    {
        [GraphQLNonNullType]
        public int bedId { get; set; }
        [GraphQLNonNullType]
        public DateTime checkInDate { get; set; }
        [GraphQLNonNullType]
        public String patientId { get; set; }
        [GraphQLNonNullType]
        public bool IsIcu { get; set; }
    }
}
