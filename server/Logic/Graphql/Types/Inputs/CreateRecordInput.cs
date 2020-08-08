using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This calss handles the record clinic new entries.
    /// </summary>
    public class CreateRecordInput
    {
        /// <summary>
        /// The patient id code.
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The pathology name of the entry.
        /// </summary>
        [GraphQLNonNullType]
        public string pathologyName { get; set; }
        /// <summary>
        /// The diagnostic date when the pathology was diagnosticated.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime? diagnosticDate { get; set; }
        /// <summary>
        /// The treatment that the patient is taking.
        /// </summary>
        public string treatment { get; set; } = null;
    }
}
