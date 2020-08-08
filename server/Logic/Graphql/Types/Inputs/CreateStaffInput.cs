using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information of the new staff member.
    /// </summary>
    public class CreateStaffInput
    {
        /// <summary>
        /// Identification code.
        /// </summary>
        [GraphQLNonNullType]
        public string id { get; set; }
        /// <summary>
        /// Role name of the staff member.
        /// </summary>
        [GraphQLNonNullType]
        public string role { get; set; }
        /// <summary>
        /// The password for the new staff member.
        /// </summary>
        [GraphQLNonNullType]
        public string password { get; set; }
        /// <summary>
        /// The admission date of the new staff member.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime admissionDate { get; set; }
    }
}
