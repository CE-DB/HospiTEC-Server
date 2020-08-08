using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to add procedures to specific clinic record.
    /// </summary>
    public class CreateAppointmentInput
    {
        /// <summary>
        /// The patient identification code.
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The name of the specific pathology in the clinic record entry.
        /// </summary>
        [GraphQLNonNullType]
        public string pathologyName { get; set; }
        /// <summary>
        /// The name of the medical procedure to add.
        /// </summary>
        [GraphQLNonNullType]
        public string procedureName { get; set; }
        /// <summary>
        /// The date when the diagnostic was made.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime? diagnosticDate { get; set; }
        /// <summary>
        /// The date when the medical procedure was executed.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime? executionDate { get; set; }

    }
}
