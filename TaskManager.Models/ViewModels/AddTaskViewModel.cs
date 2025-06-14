using System.ComponentModel.DataAnnotations;
using TaskManager.Models.Constants;

namespace TaskManager.Models.ViewModels
{
    public class AddTaskViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title can't be more than 200 words")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description can't be more that 1000 words")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public Models.Constants.TaskStatus Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public Priority Priority { get; set; }

        [Required(ErrorMessage = "Severity is required")]
        public Severity Severity { get; set; }

        [Display(Name = "Is Parent?")]
        public bool IsParent { get; set; }

        [Display(Name = "Assigned To")]
        public string UserId { get; set; }

        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Display(Name = "Parent Task")]
        public int? ParentTaskId { get; set; }
    }
}
