using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateRecordInput
    {
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public string oldPathologyName { get; set; }
        [GraphQLNonNullType]
        public DateTime? oldDiagnosticDate { get; set; }
        public string treatment { get; set; } = null;
        public string newPathologyName { get; set; } = null;
        public DateTime? newDiagnosticDate { get; set; } = null;
    }
}
