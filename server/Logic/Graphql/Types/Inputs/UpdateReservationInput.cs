using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateReservationInput
    {
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public DateTime oldCheckInDate { get; set; }
        public DateTime? newCheckInDate { get; set; } = null;
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> newProcedures { get; set; } = null;
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> oldProcedures { get; set; } = null;
    }
}
