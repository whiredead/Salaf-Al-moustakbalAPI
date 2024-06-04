using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPfe.DTOs;
using ProjectPfe.DTOs.Account;
using ProjectPfe.Models;
using ProjectPfe.Services;
using System.IO;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectPfe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private IWebHostEnvironment _env;
        private readonly Service _service;

        public AppController(Service service,JWTService jwtService, SignInManager<User> _signInManager, UserManager<User> _userManager, IWebHostEnvironment env)
        {

            this._jwtService = jwtService;
            this._signInManager = _signInManager;
            this._userManager = _userManager;
            this._env = env;
            this._service= service;
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
            try {
                var user = await this._userManager.FindByNameAsync(loginDto.Email);
                if (user == null) {
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

                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;
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

        [Authorize]
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
        [HttpGet("get-bars")]
        public async Task<ActionResult<ICollection<BarDto>>> GetBarsAsync()
        {
            List<BarDto> barsDto = await _service.GetBarsAsync();
            
            return Ok(barsDto);
        }

        [Authorize]
        [HttpGet("get-bar/{id}")]
        public async Task<ActionResult<BarDto>> GetBarAsync(int id)
        {
            BarDto barDto = await _service.GetBarAsyncById(id);
            if (barDto == null)
            {
                return NotFound();
            }

            return Ok(barDto);
        }
    }
}