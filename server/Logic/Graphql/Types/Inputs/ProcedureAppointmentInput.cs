using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information for specifid procedures in specific clinic record.
    /// </summary>
    public class ProcedureAppointmentInput
    {
        /// <summary>
        /// The name of the procedure.
        /// </summary>
        [GraphQLNonNullType]
        public string procedureName { get; set; }
        /// <summary>
        /// The execution date when the procedure was made.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime executionDate { get; set; }
    }
}
