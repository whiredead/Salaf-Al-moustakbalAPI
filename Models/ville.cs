using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalafAlmoustakbalAPI.Models
{
    public class ville
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }

    }
}
