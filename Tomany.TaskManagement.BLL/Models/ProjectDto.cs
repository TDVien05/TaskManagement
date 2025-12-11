using System;

namespace Tomany.TaskManagement.BLL.Models
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? ProjectStatus { get; set; }
        public string? CreatedByUsername { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
