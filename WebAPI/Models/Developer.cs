using System;
using System.Collections.Generic;

namespace Repositories.WebAPI.Models
{
    public partial class Developer
    {
        public Developer()
        {
            Project = new HashSet<Project>();
        }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Project> Project { get; set; }
    }
}
