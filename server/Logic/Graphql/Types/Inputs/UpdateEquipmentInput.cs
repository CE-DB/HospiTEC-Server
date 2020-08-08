using HotChocolate;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to update medical equipment and locate the equipment to update.
    /// </summary>
    public class UpdateEquipmentInput
    {
        /// <summary>
        /// Serial number of the equipment to update
        /// </summary>
        [GraphQLNonNullType]
        public string oldSerialNumber { get; set; }
        /// <summary>
        /// New serial number of the equipment.
        /// </summary>
        public string newSerialNumber { get; set; } = null;
        /// <summary>
        /// The new name for the equipment
        /// </summary>
        public string name { get; set; } = null;
        /// <summary>
        /// The new stock value for the equipment
        /// </summary>
        public int? stock { get; set; } = null;
        /// <summary>
        /// The new provider name of the equipment.
        /// </summary>
        public string provider { get; set; } = null;
    }
}
