using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to add procedures to reservations.
    /// </summary>
    public class AddProcedureReservedInput
    {
        /// <summary>
        /// The name of the procedure
        /// </summary>
        [GraphQLNonNullType]
        public string name { get; set; }
        /// <summary>
        /// The patient identification number
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The check in date of the reservation.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime checkInDate { get; set; }
        /// <summary>
        /// If the bed have to be ICU or not
        /// </summary>
        [GraphQLNonNullType]
        public bool icu { get; set; }
    }
}
