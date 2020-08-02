using HotChocolate;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class CreatePersonInput
    {
        [GraphQLNonNullType]
        public string id { get; set; }
        [GraphQLNonNullType]
        public string firstName { get; set; }
        [GraphQLNonNullType]
        public string lastName { get; set; }
        [GraphQLNonNullType]
        public string phoneNumber { get; set; }
        [GraphQLNonNullType]
        public string canton { get; set; }
        [GraphQLNonNullType]
        public string province { get; set; }
        [GraphQLNonNullType]
        public string address { get; set; }
        [GraphQLNonNullType]
        public DateTime birthDate { get; set; }
    }
}
