using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketin_BlackStone.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please type your comment")]
        public string Body { get; set; }

        public DateTime CreatedDate { get; set; }

        public int TicketId { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }
        public virtual Ticket Ticket { get; set; }


    }
}
