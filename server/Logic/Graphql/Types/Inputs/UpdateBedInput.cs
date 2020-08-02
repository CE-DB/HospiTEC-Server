using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdateBedInput
    {
        [GraphQLNonNullType]
        public int oldBedId { get; set; }
        public int? newBedId { get; set; } = null;
        public bool? isIcu { get; set; } = null;
        public int? idRoom { get; set; } = null;
    }
}
