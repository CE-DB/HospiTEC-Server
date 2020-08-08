using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    /// <summary>
    /// Maps the access key and the role of the user who is logging in
    /// </summary>
    public class Auth
    {
        public string accessKey { get; set; }

        public string role { get; set; }
    }
}
