﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Branch : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public virtual ICollection<ApplicationUser> RegisteredMembers { get; set; }
        public virtual ICollection<Package> RegisteredPackages { get; set; }
    }
}
