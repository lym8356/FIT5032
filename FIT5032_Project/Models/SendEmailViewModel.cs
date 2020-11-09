using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FIT5032_Project.Models
{
    public class SendEmailViewModel
    {
        [Display(Name = "Send to: ")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "Please select at least one email address.")]
        public string ToEmail { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter the contents")]
        public string Contents { get; set; }

        [Display(Name = "Attachment")]
        public byte[] Attachment { get; set; }
    }
}