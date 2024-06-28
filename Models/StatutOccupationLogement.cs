using System.ComponentModel.DataAnnotations;

namespace SalafAlmoustakbalAPI.Models
{
    public class StatutOccupationLogement
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
    }
}
