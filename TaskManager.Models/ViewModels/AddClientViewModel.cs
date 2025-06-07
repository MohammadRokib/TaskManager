using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels
{
    public class AddClientViewModel
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Full Name should be under 100 characters")]
        [Display(Name = "Full Name")]
        public string ClientFullName { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Full Name should be under 100 characters")]
        [Display(Name = "Short Name")]
        public string ClientShortName { get; set; }

        [Display(Name = "Address")]
        public string ClientAddress { get; set; }
    }
}
