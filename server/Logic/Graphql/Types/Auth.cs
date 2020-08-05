using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospiTec_Server.Logic.Graphql.Types
{
    public class Auth
    {
        public string accessKey { get; set; }

        public string role { get; set; }
    }
}
