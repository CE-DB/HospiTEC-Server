using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information for new medical room.
    /// </summary>
    public class CreateMedicalRoomInput
    {
        /// <summary>
        /// The id number of the new medical room.
        /// </summary>
        [GraphQLNonNullType]
        public int id { get; set; }
        /// <summary>
        /// The floor number where the medical room is located.
        /// </summary>
        [GraphQLNonNullType]
        public short floorNumber { get; set; }
        /// <summary>
        /// The name of the new medical room.
        /// </summary>
        [GraphQLNonNullType]
        public string name { get; set; }
        /// <summary>
        /// The bedds capacity of the new medical room.
        /// </summary>
        [GraphQLNonNullType]
        public short capacity { get; set; }
        /// <summary>
        /// The care type name of the new medical room.
        /// </summary>
        [GraphQLNonNullType]
        public string careType { get; set; }
    }
}
