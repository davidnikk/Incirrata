using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Incirrata.DataLayer;

namespace Incirrata.Models
{
    public class TaskViewModel
    {
        public string Id { get; set; }
        public string Status { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }
        [Required]
        public string Description { get; set; }
        public int Progress { get; set; }
        public string User { get; set; }
        public string ProjectCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        public List<AspNetUser> Developers { get; set; }

        public int[] progressValues { get; set; }

        public string[] statusValues { get; set; }

        public List<ProjectViewModel> Projects { get; set; }
        public string CurrentDeveloperId { get; set; }
        public string CurrentProject { get; set; }

        public string CurrentDeveloper { get; set; }
        public List<UserViewModelDDL> UsersDDL { get; set; }

        public TaskViewModel()
        {
            this.statusValues = new string[]{ "New", "In Progress", "Finished"};
            this.progressValues = new int[] { 0, 50, 100 };
        }
    }
}