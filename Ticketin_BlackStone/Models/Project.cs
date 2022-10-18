using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ticketin_BlackStone.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }


    }
}
