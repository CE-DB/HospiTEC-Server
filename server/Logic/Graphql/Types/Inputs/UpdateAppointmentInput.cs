using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateAppointmentInput
    {
        [GraphQLNonNullType]
        public string patientId { get; set; }
        [GraphQLNonNullType]
        public string pathologyName { get; set; }
        [GraphQLNonNullType]
        public DateTime diagnosticDate { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<ProcedureAppointmentInput> deletedProcedures { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<ProcedureAppointmentInput> newProcedures { get; set; }
    }
}
