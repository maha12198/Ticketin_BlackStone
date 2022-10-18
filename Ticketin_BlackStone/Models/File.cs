using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketin_BlackStone.Models
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Path { get; set; }

        [ForeignKey("FK_Files_Tickets_TicketId")]
        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }



    }
}
