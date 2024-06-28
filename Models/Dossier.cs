using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalafAlmoustakbalAPI.Models
{
    public class Dossier
    {
        [Key]
        public int Id { get; set; }
        public DateOnly dateOp { get; set; }
        public DateOnly premiereEcheance { get; set; }
        public DateOnly derniereEcheance { get; set; }
        public DateOnly echeance { get; set; }
        public string credit { get; set; }
        public string periodicite { get; set; }
        public string duree {  get; set; }
        public string reference { get; set; }
        public string produit { get; set; }
        public string agence { get; set; }
        public string differe { get; set;}
        public string assurance { get; set; }
        public string codeComptable { get; set;}
        public string? cession { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client client { get; set; }

        public string UserId { get; set; }
        public User user { get; set; }
    }
}
