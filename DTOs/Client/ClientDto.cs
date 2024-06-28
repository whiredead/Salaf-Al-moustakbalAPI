namespace SalafAlmoustakbalAPI.DTOs.Client
{
    public class ClientDto
    {
        public string? codeClient { get; set; }
        public string? nom { get; set; }
        public string? prenom { get; set; }
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
        public string? statutOccupationLogement { get; set; }

    }
}
