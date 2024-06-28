using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalafAlmoustakbalAPI.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string? codeClient { get; set; }
        public string? nom { get; set; }
        public string? prenom { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly? dateRelation { get; set; }
        public DateOnly? dateNaissance { get; set; }
        public string? lieuNaissance { get; set; }
        public string? civilite { get; set; }
        public string? sitFamiliale { get; set; }
        public int? nombreEnfants { get; set; }
        public string? cin { get; set; }
        public DateOnly? dateDelivrance { get; set; }
        public string? codeImputation { get; set; }
        public string? telephone { get; set; }

        //public string? statutOccupationLogement { get; set; }
        public Domicile? domicile { get; set; }
        public int? domicileId { get; set; }

        public StatutOccupationLogement? statutOccupationLogement { get; set; }
        public int? StatutOccupationLogementId { get; set; }
        public string? statut_des {  get; set; }

        public ICollection<Dossier> dossiers { get; }

    }
}
