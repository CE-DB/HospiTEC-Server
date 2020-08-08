using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateReservationCheckInDate
    {
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public DateTime oldCheckInDate { get; set; }
        [GraphQLNonNullType]
        public DateTime newCheckInDate { get; set; }
        [GraphQLNonNullType]
        public bool icu { get; set; }
    }
}
