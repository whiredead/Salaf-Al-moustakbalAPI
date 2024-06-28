using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SalafAlmoustakbalAPI.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Le nom doit comporter au moins {2} caractères et au maximum {1} caractères.")]
        public string Nom { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Le prenom doit comporter au moins {2} caractères et au maximum {1} caractères.")]
        public string Prenom { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z_]{2,3}$", ErrorMessage = "email invalide")]
        public string Email { get; set; }

        public DateOnly? DateNaissance { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Le mot de passe doit comporter au moins {2} caractères et au maximum {1} caractères.")]
        public string Password { get; set; }
        public bool? passwordChanged { get; set; }

        [Required]
        public string Cin { get; set; }
        [Required]
        public string Genre { get; set; } // 1 male 0 female

        public string? cinAnc { get; set; }

        public IFormFile? Profile { get; set; }

        public string? ProfileContent { get; set; } // Byte array for profile image content
        public string? ProfileContentUrl { get; set; } 
        public string? passwordAnc { get; set; }
        public string? emailAnc { get; set; }

        public string? Profession { get; set; }

        public string? Adresse { get; set; }

        
        
    }
}
