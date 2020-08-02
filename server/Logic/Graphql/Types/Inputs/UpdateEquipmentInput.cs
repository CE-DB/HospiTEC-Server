using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateEquipmentInput
    {
        [GraphQLNonNullType]
        public string oldSerialNumber { get; set; }
        public string newSerialNumber { get; set; } = null;
        public string name { get; set; } = null;
        public int? stock { get; set; } = null;
        public string provider { get; set; } = null;
    }
}
