using HotChocolate;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This calss handles the information to create new medical equipment
    /// </summary>
    public class CreateEquipmentInput
    {
        /// <summary>
        /// The serial number of the new equipment.
        /// </summary>
        [GraphQLNonNullType]
        public string serialNumber { get; set; }
        /// <summary>
        /// The name of the equipment.
        /// </summary>
        [GraphQLNonNullType]
        public string name { get; set; }
        /// <summary>
        /// The stock available of this equipment.
        /// </summary>
        [GraphQLNonNullType]
        public int stock { get; set; }
        /// <summary>
        /// The provider name of the equipment
        /// </summary>
        [GraphQLNonNullType]
        public string provider { get; set; }
    }
}
