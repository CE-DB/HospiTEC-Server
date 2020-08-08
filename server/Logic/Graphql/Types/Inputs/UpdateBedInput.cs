using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handle the information to change and identify a specific bed.
    /// </summary>
    public class UpdateBedInput
    {
        /// <summary>
        /// The bed id number to update.
        /// </summary>
        [GraphQLNonNullType]
        public int oldBedId { get; set; }
        /// <summary>
        /// The new bed id number.
        /// </summary>
        public int? newBedId { get; set; } = null;
        /// <summary>
        /// Check for the bed if is for internal care unite or not.
        /// </summary>
        public bool? isIcu { get; set; } = null;
        /// <summary>
        /// The number of the room where the bed is located.
        /// </summary>
        public int? idRoom { get; set; } = null;
    }
}
