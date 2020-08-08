using HotChocolate;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles information for update the medical room.
    /// </summary>
    public class UpdateMedicalRoomInput
    {
        /// <summary>
        /// The id number of the room to update.
        /// </summary>
        [GraphQLNonNullType]
        public int oldId { get; set; }
        /// <summary>
        /// The new id number to put in the medical room
        /// </summary>
        public int? newId { get; set; } = null;
        /// <summary>
        /// The new floor number of the room
        /// </summary>
        public short? floorNumber { get; set; } = null;
        /// <summary>
        /// The new name of the room
        /// </summary>
        public string name { get; set; } = null;
        /// <summary>
        /// The new capacity value for the room
        /// </summary>
        public short? capacity { get; set; } = null;
        /// <summary>
        /// The new care type name for the room
        /// </summary>
        public string careType { get; set; } = null;
    }
}
