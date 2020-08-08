using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the new person information
    /// </summary>
    public class CreatePersonInput
    {
        /// <summary>
        /// Identification code.
        /// </summary>
        [GraphQLNonNullType]
        public string id { get; set; }
        /// <summary>
        /// First name
        /// </summary>
        [GraphQLNonNullType]
        public string firstName { get; set; }
        /// <summary>
        /// Last name
        /// </summary>
        [GraphQLNonNullType]
        public string lastName { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        [GraphQLNonNullType]
        public string phoneNumber { get; set; }
        /// <summary>
        /// Canton
        /// </summary>
        [GraphQLNonNullType]
        public string canton { get; set; }
        /// <summary>
        /// Province.
        /// </summary>
        [GraphQLNonNullType]
        public string province { get; set; }
        /// <summary>
        /// Address.
        /// </summary>
        [GraphQLNonNullType]
        public string address { get; set; }
        /// <summary>
        /// Birth date.
        /// </summary>
        [GraphQLNonNullType]
        public DateTime birthDate { get; set; }
    }
}
