using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Incirrata.DataLayer;

namespace Incirrata.Models
{
    public class ProjectViewModel
    {
        public string Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s_.\d]+$", ErrorMessage = "Use letters and number only.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AspNetUser> ProjectManagers { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public string CurrentProjectManagerId { get; set; }
        public string CurrentProjectManager { get; set; }
    }
}