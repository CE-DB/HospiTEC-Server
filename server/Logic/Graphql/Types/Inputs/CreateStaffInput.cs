using HotChocolate;
using System;

namespace HospiTec_Server.Logic.Graphql.Types.Inputs
{
    public class CreateStaffInput
    {
        [GraphQLNonNullType]
        public string role { get; set; }
        [GraphQLNonNullType]
        public string password { get; set; }
        [GraphQLNonNullType]
        public DateTime admissionDate { get; set; }
    }
}
