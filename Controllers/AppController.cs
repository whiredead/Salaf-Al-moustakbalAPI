using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalafAlmoustakbalAPI.DTOs;
using SalafAlmoustakbalAPI.DTOs.Client;
using SalafAlmoustakbalAPI.DTOs.Account;
using SalafAlmoustakbalAPI.Models;
using SalafAlmoustakbalAPI.Services;
using System.IO;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Security.AccessControl;

namespace SalafAlmoustakbalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly Service _service;

        public AppController(Service service)
        {
            this._service= service;
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

        [HttpPost("checkClient/{code}")]
        public async Task<IActionResult> CheckClientAsync(string code)
        {
            bool clientExists = await _service.CheckClientAsync(code);

            return Ok(clientExists);
        }
        /*
        [Authorize]
        [HttpPost("creerClient")]
        public async Task<IActionResult> CreerClient([FromForm] CreationClient client)
        {
            Console.WriteLine("######client ceation");

            try
            {
                Console.WriteLine($"####### Received client data: civilite={client.civilite}, codeclient={client.codeClient}, ...");

                if (await this._service.CheckClientAsync(client.codeClient))
                {
                    return BadRequest(new JsonResult(new { title = "creation echoue", message = "Un client existant utilise le meme code client." }));
                }
                else
                {

                    // Call the service method to create the client
                    await this._service.CreerClient(client);

                    // Return a success response
                    return Ok(client);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("###### "+ex.ToString());
                return StatusCode(500, "An unexpected error occurred while processing the request.");

            }
        }

        [Authorize]
        [HttpPost("creerDomicile")]
        public async Task<IActionResult> creerDomicile([FromForm] DomicileDto domicile)
        {
            Console.WriteLine($"############ Received domicile data: adresse={domicile.adresse}, ville={domicile.ville}, codeclient={domicile.codeClient}");
            await this._service.CreerDomicile(domicile);
            return Ok(domicile);

        }*/


        [Authorize]
        [HttpPost("creerClient1")]
        public async Task<IActionResult> CreerClient1([FromForm] CreationClient client)
        {
            Console.WriteLine("######client ceation");

            try
            {
                Console.WriteLine($"####### Received client data: civilite={client.civilite}, codeclient={client.codeClient}, ...");

                if (await this._service.CheckClientAsync(client.codeClient))
                {
                    return BadRequest(new JsonResult(new { title = "creation echoue", message = "Un client existant utilise le meme code client." }));
                }
                else
                {

                    // Call the service method to create the client
                    await this._service.CreerClient1(client);

                    // Return a success response
                    return Ok(client);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("###### " + ex.ToString());
                return StatusCode(500, "An unexpected error occurred while processing the request.");

            }
        }

        [Authorize]
        [HttpGet("getAllClients")]
        public async Task<IActionResult> getAllClients()
        {
            List<CreationClient> clients = await _service.GetAllClients();

            return Ok(clients);
        }

        [Authorize]
        [HttpGet("getAllVilles")]
        public async Task<IActionResult> getAllVilles()
        {
            List<villeDto> villes = await _service.GetAllVilles();

            return Ok(villes);
        }

        [Authorize]
        [HttpGet("getAllStatutOccupation")]
        public async Task<IActionResult> getAllStatutOccupation()
        {
            List<StatutOccupationDto> status = await _service.getAllStatutOccupation();

            return Ok(status);
        }

        [Authorize]
        [HttpPost("creerDossier")]
        public async Task<IActionResult> creerDossier([FromForm] DossierDto dossier)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("#### Inside CreerDossier");

            if (await this._service.CheckDossierAsync(dossier.reference))
            {
                return BadRequest(new JsonResult(new { title = "creation echoue", message = "un dossier existe a la meme référence." }));
            }
            else
            {

                var result = await this._service.creerDossier(dossier,userId);

                if (result)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500, "An error occurred while creating dossier.");
                }
            }
        }

        [Authorize]
        [HttpGet("getClientBycode/{codeClient}")]
        public async Task<ActionResult<CreationClient>> getClientBycode(string codeClient)
        {
            if (await _service.CheckClientAsync(codeClient))
            {
                CreationClient client = await _service.getClientBycode(codeClient);
                return Ok(client);

            }
            else
            {
                return BadRequest(new JsonResult(new { title = "Opération échouée", message = "Il n'y a aucun client avec ce code" }));

            }
        }
        
        [Authorize]
        [HttpPost("upateClient")]
        public async Task<IActionResult> UpdateAsync([FromBody] CreationClient data)
        {
            if (await _service.CheckClientAsync(data.codeClient))
            {
                if(await this._service.updateClient(data)) { 
                    return Ok(new JsonResult(new { title = "Update successful" })); 
                }
                else
                {
                    return BadRequest(new JsonResult(new { title = "Opération échouée" }));

                }
            }
            else
            {
                return BadRequest(new JsonResult(new { title = "Opération échouée", message = "Il n'y a aucun client avec ce code" }));

            }
            
        }
        [Authorize]
        [HttpGet("getClientByLabel/{label}")]
        public async Task<ActionResult<List<CreationClient>>> getClientByLabel(string label)
        {
            var c = await _service.getClientByLabel(label);
            if (c != null) { 
                Console.WriteLine("##### " + label);
                return c;
            }
            else
            {
                return BadRequest(new JsonResult(new { title = "Opération échouée" }));


            }
        }

        [Authorize]
        [HttpGet("getAllDossiers")]
        public async Task<IActionResult> getAllDossiers()
        {
            List<DossierDto> dossiers = await _service.getAllDossiers();

            return Ok(dossiers);
        }
        [Authorize]
        [HttpGet("getDossierByLabel/{label}")]
        public async Task<ActionResult<List<DossierDto>>> getDossierByLabel(string label)
        {
            var c = await _service.getDossiersByLabel(label);
            if (c != null)
            {
                Console.WriteLine("##### " + label);
                return c;
            }
            else
            {
                return BadRequest(new JsonResult(new { title = "Opération échouée" }));


            }
        }

        [Authorize]
        [HttpGet("getDossierByReference/{reference}")]
        public async Task<ActionResult<DossierDto>> getDossierByReference(string reference)
        {
            if (await _service.CheckDossierAsync(reference))
            {
                DossierDto dossier = await _service.getDossierByReference(reference);
                return Ok(dossier);

            }
            else
            {
                return BadRequest(new JsonResult(new { title = "Opération échouée", message = "Il n'y a aucun client avec ce code" }));

            }
        }

        [Authorize]
        [HttpPost("updateDossier")]
        public async Task<IActionResult> updateDossier([FromForm] DossierDto dossier)
        {
            if (await _service.CheckDossierAsync(dossier.reference))
            {
               if(await _service.updateDossier(dossier))
               {
                    return Ok(new JsonResult(new { title = "Update successful" ,message= "le dossier est modifié avec succes" }));

               }
                else
                {
                    return BadRequest(new JsonResult(new { title = "Opération échouée", message = "Opération échouée" }));

                }
            }
            else
            {
                return BadRequest(new JsonResult(new { title = "Opération échouée", message = "Il existe un dossier avec la meme reference " }));

            }
        }


    }
}