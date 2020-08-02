using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class Role
    {
        public Role()
        {
            Staff = new HashSet<Staff>();
        }

        public string Name { get; set; }

        public  ICollection<Staff> Staff { get; set; }
    }
}
