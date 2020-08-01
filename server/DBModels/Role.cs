using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class Role
    {
        public Role()
        {
            Staff = new HashSet<Staff>();
        }

        public string Name { get; set; }

        public virtual ICollection<Staff> Staff { get; set; }
    }
}
