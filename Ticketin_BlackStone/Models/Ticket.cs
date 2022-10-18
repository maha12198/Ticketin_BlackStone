using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ticketin_BlackStone.Models
{
    public class Ticket
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string Description { get; set; }

        [Display(Name = "Project Name")]
        public int ProjectId { get; set; }

        [Required]
        public bool Status { get; set; }

        [Display(Name = "Closing Reason")]
        public string ClosingReason { get; set; }
        
        public string UserId { get; set; }

        public IdentityUser User { get; set; }

        public virtual Project Project { get; set; }


        public IList<Comment> Comments { get; set; }

        public IList<File> Files { get; set; }



    }
}
