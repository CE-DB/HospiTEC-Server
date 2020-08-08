using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to add new reservations.
    /// </summary>
    public class AddReservationInput
    {
        /// <summary>
        /// the identification code of the patient.
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The check in date of the reservation. Format (YYY-MM-DD)
        /// </summary>
        [GraphQLNonNullType]
        public DateTime checkInDate { get; set; }
    }
}
