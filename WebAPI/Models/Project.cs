using System;
using System.Collections.Generic;

namespace Repositories.WebAPI.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string IdDeveloper { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Languages { get; set; }

        public virtual Developer IdDeveloperNavigation { get; set; }
    }
}
