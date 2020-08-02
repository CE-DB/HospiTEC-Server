using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class CreateMedicalRoomInput
    {
        [GraphQLNonNullType]
        public int id { get; set; }
        [GraphQLNonNullType]
        public short floorNumber { get; set; }
        [GraphQLNonNullType]
        public string name { get; set; }
        [GraphQLNonNullType]
        public short capacity { get; set; }
        [GraphQLNonNullType]
        public string careType { get; set; }
    }
}
