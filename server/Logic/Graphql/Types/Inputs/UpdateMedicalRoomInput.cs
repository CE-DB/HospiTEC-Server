using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateMedicalRoomInput
    {
        [GraphQLNonNullType]
        public int oldId { get; set; }
        public int? newId { get; set; } = null;
        public short? floorNumber { get; set; } = null;
        public string name { get; set; } = null;
        public short? capacity { get; set; } = null;
        public string careType { get; set; } = null;
    }
}
