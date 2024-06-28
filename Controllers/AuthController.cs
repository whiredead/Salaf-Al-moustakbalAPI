using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using SalafAlmoustakbalAPI.Data;
using SalafAlmoustakbalAPI.DTOs.Account;
using SalafAlmoustakbalAPI.DTOs.Client;
using SalafAlmoustakbalAPI.Models;
using SalafAlmoustakbalAPI.Services;
using System.Diagnostics.Metrics;
using System.IO;
using System.Security.Claims;

namespace SalafAlmoustakbalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private IWebHostEnvironment _env;
        private readonly IdentityContext _context;

        private readonly Service _service;

        public AuthController(Service service, JWTService jwtService, SignInManager<User> _signInManager, UserManager<User> _userManager, IWebHostEnvironment env, IdentityContext context)
        {

            this._jwtService = jwtService;
            this._signInManager = _signInManager;
            this._userManager = _userManager;
            this._env = env;
            this._service = service;
            this._context = context;
        }


        private UserDto CreateApplicationUserDto(User user)
        {
            string profilePictureUrl = null;
            if (!string.IsNullOrEmpty(user.Profile))
            {
                profilePictureUrl = $"{Request.Scheme}://{Request.Host}/profiles/{user.Profile}";
            }

            return new UserDto
            {
                FirstName = user.Prenom,
                LastName = user.Nom,
                JWT = _jwtService.CreateJWT(user),
                ProfilePicture = profilePictureUrl,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            try
            {
                var user = await this._userManager.FindByNameAsync(loginDto.Email);
                if (user == null)
                {
                    Console.WriteLine("########Nom d'utilisateur ou mot de passe invalide mot");
                    return BadRequest(new JsonResult(new { title = "connexion echoue", message = "Nom d'utilisateur ou mot de passe invalide" }));
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded) return BadRequest(new JsonResult(new { title = "connexion echoue", message = "Nom d'utilisateur ou mot de passe invalide" }));
                UserDto userdto = CreateApplicationUserDto(user);
                Console.WriteLine("########## userdto " + userdto.ProfilePicture);
                return CreateApplicationUserDto(user);
            }
            catch (Exception ex) { return BadRequest(new JsonResult(new { title = "connexion echoue", message = ex.Message })); }

        }

        private async Task<bool> CheckUserExistsAsync(string email, string cin)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower() || x.Cin == cin.ToLower());
        }


        [HttpPost]
        public async Task<string> UploadFile(IFormFile file, string uploadFolder)
        {
            try
            {
                var result = "";
                var uploads = Path.Combine(this._env.WebRootPath, uploadFolder);
                Console.WriteLine("############ web root " + this._env.WebRootPath + "#### uploads  " + uploads);

                if (!Directory.Exists(uploads))
                {
                    Console.WriteLine("############upload folder does not exist, creating...");
                    Directory.CreateDirectory(uploads);
                }
                else
                {
                    Console.WriteLine("############upload folder already exist,");
                }

                var fileName = file.FileName.ToString();
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    Console.WriteLine($"#############File '{fileName}' copied successfully to '{filePath}'.");
                }

                // Check if the file was successfully copied
                if (System.IO.File.Exists(filePath))
                {
                    result = fileName;
                }
                else
                {
                    Console.WriteLine($"##########Failed to copy file '{fileName}' to '{filePath}'. File not found.");
                    // Optionally, you can throw an exception here or handle the failure accordingly.
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while uploading file: {ex.Message}");
                throw; // Re-throw the exception to propagate it
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            Console.WriteLine("############## register called.");

            if (await CheckUserExistsAsync(registerDto.Email, registerDto.Cin))
            {
                Console.WriteLine("############## Un compte existant utilise l'adresse e-mail ou le CIN.");
                return BadRequest(new JsonResult(new { title = "connexion echoue", message = "Un compte existant utilise l'adresse e-mail ou le CIN." }));
            }
            var userToAdd = new User
            {
                Nom = registerDto.Nom.ToLower(),
                Prenom = registerDto.Prenom.ToLower(),
                Cin = registerDto.Cin.ToLower(),
                Genre = registerDto.Genre,
                UserName = registerDto.Email.ToLower(),
                Email = registerDto.Email.ToLower(),
                DateNaissance = registerDto.DateNaissance,
                Profession = registerDto.Profession,
                Addresse = registerDto.Adresse,
                EmailConfirmed = true,
            };
            if (registerDto.Profile != null)
            {
                var fileName = await UploadFile(registerDto.Profile, @"profiles\");
                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("############## Failed to upload profile picture.");
                    return BadRequest(new JsonResult(new { title = "inscription echoue", message = "Failed to upload profile picture." }));

                }

                userToAdd.Profile = fileName;
            }
            var result = await _userManager.CreateAsync(userToAdd, registerDto.Password);
            if (!result.Succeeded)
            {
                Console.WriteLine(" result ############## " + result.Errors);
                return BadRequest(new JsonResult(new { title = "inscription echoue", message = result.Errors }));
            }

            try
            {
                return Ok(new JsonResult(new { title = "compte cree", message = "Votre compte a été créé avec succès" }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(" execption ############## " + ex.Message);
                return BadRequest(new JsonResult(new { title = "inscription echoue", message = ex.Message }));
            }
        }

        [Authorize] // expect an jwt
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            Console.WriteLine("###### inside refresh token dotnet");
            return CreateApplicationUserDto(user);
        }

        [Authorize]
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<List<RegisterDto>>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();

            // Using LINQ Select to asynchronously create RegisterDto objects
            var registerDtos = await Task.WhenAll(users.Select(async user =>
            {
                var userDto = new RegisterDto
                {
                    Adresse=user.Addresse,
                    Cin = user.Cin,
                    Nom = user.Nom,
                    Prenom = user.Prenom,
                    Email = user.Email,
                    DateNaissance = user.DateNaissance,
                    Profession = user.Profession,
                    Genre = user.Genre,
                };

                if (user.Profile != null)
                {
                    userDto.ProfileContentUrl = user.Profile;
                    userDto.ProfileContent = await GetProfileImageAsync($"profiles/{user.Profile}");
                }

                return userDto;
            }));

            return Ok(registerDtos.ToList());
        }


        [Authorize]
        [HttpGet("getUtilisateurByCin/{cin}")]
        public async Task<ActionResult<RegisterDto>> GetUtilisateurByCin(string cin)
        {
            if (string.IsNullOrEmpty(cin))
            {
                return BadRequest("CIN cannot be null or empty.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Cin.ToLower() == cin.ToLower());
            var userDto = new RegisterDto
            {
                Adresse = user.Addresse,
                Cin = user.Cin,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                DateNaissance = user.DateNaissance,
                Password = user.PasswordHash,
                Profession = user.Profession,
                Genre = user.Genre,

            };
            if (user.Profile != null)
            {
                //user.Profile = $"{Request.Scheme}://{Request.Host}/profiles/{user.Profile}";
                Console.WriteLine("pofile use " + user.Profile);

                userDto.ProfileContentUrl = user.Profile;
                Console.WriteLine(userDto.ProfileContentUrl);
                userDto.ProfileContent = await GetProfileImageAsync($"profiles/{user.Profile}");
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userDto);
        }

        [HttpGet("Profile/{filename}")]
        public async Task<IActionResult> GetFile(string filename)
        {
            var filePath = Path.Combine(this._env.WebRootPath, "profiles", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" }
        };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.GetValueOrDefault(ext, "application/octet-stream");
        }

        [Authorize]
        [HttpGet("searchByLabel/{label}")]
        public async Task<ActionResult<List<RegisterDto>>> SearchByLabel(string label)
        {
            Console.WriteLine("######## " + label);
            if (label == "homme")
            {
                label = "1";
            }
            if (label == "femme")
            {
                label = "0";
            }

            var users = await _userManager.Users
                .Where(u =>
                    u.Genre.Contains(label) ||
                    u.Nom.Contains(label) ||
                    u.Prenom.Contains(label) ||
                    u.Cin.Contains(label) ||
                    u.Email.Contains(label) ||
                    u.Addresse.Contains(label) ||
                    u.Profession.Contains(label))
                .Select(u => new User
                {
                    Nom = u.Nom,
                    Prenom = u.Prenom,
                    Email = u.Email,
                    DateNaissance = u.DateNaissance,
                    Cin = u.Cin,
                    Genre = u.Genre,
                    Profession = u.Profession,
                    Addresse = u.Addresse,
                    Profile = u.Profile,
                })
                .ToListAsync();

            if (users != null && users.Count > 0)
            {
                var registerDtoList = new List<RegisterDto>();

                foreach (var u in users)
                {
                    var registerDto = new RegisterDto
                    {
                        Nom = u.Nom,
                        Prenom = u.Prenom,
                        Email = u.Email,
                        DateNaissance = u.DateNaissance,
                        Cin = u.Cin,
                        Genre = u.Genre,
                        Profession = u.Profession,
                        Adresse = u.Addresse,
                    };

                    if (!string.IsNullOrEmpty(u.Profile))
                    {
                        Console.WriteLine(u.Nom);
                        // Get profile image as byte array
                        registerDto.ProfileContent = await GetProfileImageAsync($"profiles/{u.Profile}");
                        registerDto.ProfileContentUrl = $"{Request.Scheme}://{Request.Host}/profiles/{u.Profile}";
                    }

                    registerDtoList.Add(registerDto);
                }

                return registerDtoList;
            }
            return null;
        }

        private async Task<string> GetProfileImageAsync(string profileFileName)
        {
            try
            {
                Console.WriteLine("## " + profileFileName);

                var filepath = Path.Combine(this._env.WebRootPath, profileFileName);
                Console.WriteLine("## exist  " + filepath);

                // Adjust this path to match your actual profile image storage location
                if (System.IO.File.Exists(filepath))
                {
                    Console.WriteLine("## exist  " + filepath);

                    var bye = await System.IO.File.ReadAllBytesAsync(filepath);
                    string base64ImageString = "data:image/;base64," + Convert.ToBase64String(bye);
                    return base64ImageString;
                }
                return null; // Handle case where file doesn't exist
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error retrieving profile image: {ex.Message}");
                return null;
            }
        }
        
        [NonAction]
        public IActionResult WriteBase64ToFile(string base64String, string filePath)
        {
            Console.WriteLine(' '+ base64String + ' ' + filePath);

            try
            {
                byte[] byteArray = Convert.FromBase64String(base64String);

                string relativePath = Path.Combine("profiles", filePath);
                System.IO.File.WriteAllBytes(relativePath, byteArray);
                Console.WriteLine("File created successfully at " + filePath);

                return Ok("File created successfully");
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Invalid base64 string: " + ex.Message);
                return BadRequest("Invalid base64 string: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("IO error while writing the file: " + ex.Message);
                return StatusCode(500, "IO error while writing the file: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        [HttpDelete("deleteFile")]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                string relativePath = Path.Combine("profiles", fileName);
                string filePath = Path.Combine(_env.WebRootPath, relativePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok("File deleted successfully");
                }
                else
                {
                    return NotFound("File not found");
                }
            }
            catch (IOException ex)
            {
                return StatusCode(500, "IO error while deleting the file: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] RegisterDto formData)
        {
    
            Console.WriteLine("inside update " + formData.emailAnc + ' ' + formData.cinAnc + ' ' + formData.ProfileContentUrl+' '+ formData.ProfileContent);

            if (await CheckUserExistsAsync(formData.emailAnc, formData.cinAnc))
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == formData.emailAnc.ToLower() || x.Cin == formData.cinAnc.ToLower());
                if (user == null)
                {
                    return BadRequest("User does not exist.");
                }

                user.Nom = formData.Nom;
                user.Prenom = formData.Prenom;
                user.Genre = formData.Genre;
                user.Addresse = formData.Adresse;
                user.Profession = formData.Profession;
                user.DateNaissance = formData.DateNaissance;

                var userCin = await _userManager.Users.FirstOrDefaultAsync(x => x.Cin == formData.Cin.ToLower() && x.Id != user.Id);
                if (userCin != null)
                {
                    return BadRequest(new { title = "Modification echouée", message = "Un compte existant utilise le CIN." });
                }
                else
                {
                    user.Cin = formData.Cin;
                }

                var userEmail = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == formData.Email.ToLower() && x.Id != user.Id);
                if (userEmail != null)
                {
                    return BadRequest(new { title = "Modification echouée", message = "Un compte existant utilise l'email." });
                }
                else
                {
                    user.UserName = formData.Email.ToLower();
                    user.Email = formData.Email.ToLower();
                }

                if (!string.IsNullOrEmpty(user.Profile))
                {
                    DeleteFile(user.Profile);
                }

                user.Profile =null;

                if (formData.Profile!=null)
                {
                    Console.WriteLine("'############'" +formData.ProfileContentUrl);
                    user.Profile = formData.ProfileContentUrl;
                    UploadFile(formData.Profile, @"profiles\");
                }

                if (!string.IsNullOrEmpty(formData.Password) && formData.passwordChanged == true)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result1 = await _userManager.ResetPasswordAsync(user, token, formData.Password);

                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to update user.");
                }
                return Ok(new JsonResult(new { title = "User updated successfully.", message = "User updated successfully." }));
            }
            return BadRequest(new { title = "Modification echouée", message = "User does not exist." });
        }
    }
}
