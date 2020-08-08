using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to update a specific clinic record.
    /// </summary>
    public class UpdateRecordInput
    {
        /// <summary>
        /// The patient identification code related with the record.
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The pathology name of the record to update.
        /// </summary>
        [GraphQLNonNullType]
        public string oldPathologyName { get; set; }
        /// <summary>
        /// The old diagnostic date of the record to update.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime? oldDiagnosticDate { get; set; }
        /// <summary>
        /// The new treatment.
        /// </summary>
        public string treatment { get; set; } = null;
        /// <summary>
        /// The new pathology name.
        /// </summary>
        public string newPathologyName { get; set; } = null;
        /// <summary>
        /// The new diagnostic date
        /// </summary>
        public DateTime? newDiagnosticDate { get; set; } = null;
    }
}
