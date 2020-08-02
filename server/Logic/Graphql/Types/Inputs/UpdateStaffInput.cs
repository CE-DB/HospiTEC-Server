using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateStaffInput
    {
        [GraphQLNonNullType]
        public string id { get; set; }
        [GraphQLNonNullType]
        public string oldRole { get; set; }
        public string newRole { get; set; } = null;
        public DateTime? admissionDate { get; set; } = null;
        public string password { get; set; } = null;
    }
}
