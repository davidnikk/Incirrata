namespace Incirrata.DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Task")]
    public partial class Task
    {
        public string Id { get; set; }

        public DateTime Deadline { get; set; }

        [Required]
        public string Description { get; set; }

        public int Progress { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string ProjectId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        [StringLength(128)]
        public string CreatorId { get; set; }

        [StringLength(128)]
        public string Status { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual AspNetUser AspNetUser1 { get; set; }

        public virtual Project Project { get; set; }
    }
}
