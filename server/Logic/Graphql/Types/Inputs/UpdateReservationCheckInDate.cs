using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to change the check in date of specific reservation
    /// </summary>
    public class UpdateReservationCheckInDate
    {
        /// <summary>
        /// The patient identification code
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The check in date of the reservation to update.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime oldCheckInDate { get; set; }
        /// <summary>
        /// The new check in date.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime newCheckInDate { get; set; }
        /// <summary>
        /// Updates the bed to change it for a intensive care unite.
        /// </summary>
        [GraphQLNonNullType]
        public bool icu { get; set; }
    }
}
