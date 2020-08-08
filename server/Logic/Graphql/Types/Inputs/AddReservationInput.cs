using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class AddReservationInput
    {
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public DateTime checkInDate { get; set; }
    }
}
