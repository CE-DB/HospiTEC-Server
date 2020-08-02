using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class UpdatePersonInput
    {
        [GraphQLNonNullType]
        public string oldId { get; set; }
        public string newId { get; set; } = null;
        public string firstName { get; set; } = null;
        public string lastName { get; set; } = null;
        public string phoneNumber { get; set; } = null;
        public string canton { get; set; } = null;
        public string province { get; set; } = null;
        public string address { get; set; } = null;
        public DateTime? birthDate { get; set; } = null;
    }
}
