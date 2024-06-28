using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SalafAlmoustakbalAPI.Models
{
    public class User: IdentityUser
    {
        [Required]
        public String Nom { get; set; }
        [Required]
        public String Prenom { get; set; }
        [Required]
        public String Cin { get; set; }
        [Required]
        public String Genre { get; set; } // 1 male 0 female

        public String? Profile { get; set; } // picture
        
        public String? Addresse { get; set; }
        public String? Profession { get; set; }

        [Display(Name = "Date Naissance")]
        
        public DateOnly? DateNaissance { get; set; }

        public ICollection<Dossier> dossiers { get; }


    }
}
