using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to update the procedures attached to specific clinic record entry.
    /// </summary>
    public class UpdateAppointmentInput
    {
        /// <summary>
        /// The identification code of the patient.
        /// </summary>
        [GraphQLNonNullType]
        public string patientId { get; set; }
        /// <summary>
        /// The pathology name of the clinic record specified.
        /// </summary>
        [GraphQLNonNullType]
        public string pathologyName { get; set; }
        /// <summary>
        /// The date when the diagnostic was made.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime? diagnosticDate { get; set; }
        /// <summary>
        /// The procedure names to remove.
        /// </summary>
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<ProcedureAppointmentInput> deletedProcedures { get; set; }
        /// <summary>
        /// The procedure name to add.
        /// </summary>
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<ProcedureAppointmentInput> newProcedures { get; set; }
    }
}
