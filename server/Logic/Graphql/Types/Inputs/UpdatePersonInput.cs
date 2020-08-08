using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    /// <summary>
    /// This class handles the information for update a person.
    /// </summary>
    public class UpdatePersonInput
    {
        /// <summary>
        /// The identification code of the person to update.
        /// </summary>
        [GraphQLNonNullType]
        public string oldId { get; set; }
        /// <summary>
        /// The new identification code of the person.
        /// </summary>
        public string newId { get; set; } = null;
        /// <summary>
        /// Thew new first name.
        /// </summary>
        public string firstName { get; set; } = null;
        /// <summary>
        /// The new last name.
        /// </summary>
        public string lastName { get; set; } = null;
        /// <summary>
        /// The new phone number.
        /// </summary>
        public string phoneNumber { get; set; } = null;
        /// <summary>
        /// The new canton.
        /// </summary>
        public string canton { get; set; } = null;
        /// <summary>
        /// The new province.
        /// </summary>
        public string province { get; set; } = null;
        /// <summary>
        /// Thew new address.
        /// </summary>
        public string address { get; set; } = null;
        /// <summary>
        /// Thew new birth date.
        /// </summary>
        public DateTime? birthDate { get; set; } = null;
    }
}
