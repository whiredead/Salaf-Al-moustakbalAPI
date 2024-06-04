using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProjectPfe.Models
{
    public class Bar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public Boolean hasChild { get; set; }

        public ICollection<Menu> Menus { get; }
    }
}
