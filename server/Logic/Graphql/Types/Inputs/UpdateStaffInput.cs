using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information to update a staff member
    /// </summary>
    public class UpdateStaffInput
    {
        /// <summary>
        /// The identification code.
        /// </summary>
        [GraphQLNonNullType]
        public string id { get; set; }
        /// <summary>
        /// The role of the member to update.
        /// </summary>
        [GraphQLNonNullType]
        public string oldRole { get; set; }
        /// <summary>
        /// The new role to attach to member.
        /// </summary>
        public string newRole { get; set; } = null;
        /// <summary>
        /// The new admission date to attach
        /// </summary>
        public DateTime? admissionDate { get; set; } = null;
        /// <summary>
        /// The new password to add.
        /// </summary>
        public string password { get; set; } = null;
    }
}
