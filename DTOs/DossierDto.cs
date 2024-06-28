using SalafAlmoustakbalAPI.DTOs.Client;

namespace SalafAlmoustakbalAPI.DTOs
{
    public class DossierDto
    {
        public DateOnly dateOp { get; set; }
        public DateOnly premiereEcheance { get; set; }
        public DateOnly derniereEcheance { get; set; }
        public DateOnly echeance { get; set; }
        public string credit { get; set; }
        public string codeClient { get; set; }  
        public string? codeClientAnc { get; set; }
        public string periodicite { get; set; }
        public string duree { get; set; }
        public string reference { get; set; }
        public string? referenceAnc {  get; set; }
        public string produit { get; set; }
        public string agence { get; set; }
        public string differe { get; set; }
        public string assurance { get; set; }
        public string codeComptable { get; set; }
        public IFormFile? cession { get; set; }
        public string? cessionPath { get; set; }
        public string? cessionByte { get; set; }

        public string? userId { get; set; }
        public ClientDto? clientDto { get; set; }

    }
}
