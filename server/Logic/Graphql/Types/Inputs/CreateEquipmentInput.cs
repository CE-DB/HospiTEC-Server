using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class CreateEquipmentInput
    {
        [GraphQLNonNullType]
        public string serialNumber { get; set; }
        [GraphQLNonNullType]
        public string name { get; set; }
        [GraphQLNonNullType]
        public int stock { get; set; }
        [GraphQLNonNullType]
        public string provider { get; set; }
    }
}
