using System.ComponentModel.DataAnnotations;

namespace SalafAlmoustakbalAPI.Models
{
    public class Domicile
    {
        [Key]
        public int Id { get; set; }
        public string des_ville { get; set; }
        public string codePostal { get; set; }
        public string adresse { get; set; }


        public int? VilleId { get; set; }

        public ville ville { get; set; }

    }
}