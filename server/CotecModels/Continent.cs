using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Continent
    {
        public Continent()
        {
            Country = new HashSet<Country>();
        }

        public string Name { get; set; }

        public virtual ICollection<Country> Country { get; set; }
    }
}
