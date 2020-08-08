using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to update the procedures related to specific reservation.
    /// </summary>
    public class UpdateReservationProceduresInput
    {
        /// <summary>
        /// The patient identification code.
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The check in date.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime checkInDate { get; set; }
        /// <summary>
        /// Check for the bed if is icu or not.
        /// </summary>
        [GraphQLNonNullType]
        public bool icu { get; set; }
        /// <summary>
        /// The list of procedure names to add.
        /// </summary>
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> newProcedures { get; set; } = null;
        /// <summary>
        /// The list of procedure names to remove.
        /// </summary>
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> deletedProcedures { get; set; } = null;
    }
}
