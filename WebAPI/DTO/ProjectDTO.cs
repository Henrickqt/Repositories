using System;
using System.Collections.Generic;

namespace Repositories.WebAPI.DTO
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string IdDeveloper { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Languages { get; set; }
    }
}
