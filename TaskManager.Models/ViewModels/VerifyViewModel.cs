using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels
{
    public class VerifyViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The {0} must be {2} and at max {1} characters long.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}
