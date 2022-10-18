using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;  

namespace Ticketin_BlackStone.Models.ViewModels
{
    public class TicketsVM
    {
        [Display(Name = "Ticket ID")]
        public int Ticket_Id { get; set; }

        [Display(Name = "Ticket Description")]
        public string Ticket_Description { get; set; }

        [Display(Name = "Project Name")]
        public string Project_Name { get; set; }

        [Display(Name = "Status of Ticket")]
        public bool Ticket_Status { get; set; }

        [Required(ErrorMessage = "Please enter the reason of closing the ticket")]
        [Display(Name = "Closing Reason")]
        public string Ticket_ClosingReason { get; set; }

        public string User_Id { get; set; }

        public List<Comment> Comments { get; set; }

        public List<File> Files { get; set; }
      

    }
}
