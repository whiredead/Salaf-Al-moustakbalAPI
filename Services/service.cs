
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalafAlmoustakbalAPI.Data;
using SalafAlmoustakbalAPI.Models;
using SalafAlmoustakbalAPI.DTOs;
using SalafAlmoustakbalAPI.DTOs.Client;
using System.Diagnostics.Metrics;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.SqlServer.Server;
using Azure.Core;

namespace SalafAlmoustakbalAPI.Services
{

    public class Service
    {
        private readonly IConfiguration _config;

        private readonly IdentityContext _context;
        private IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;


        public Service(IdentityContext context, IWebHostEnvironment env, UserManager<User> _userManager, IConfiguration config)
        {
            _context = context;
            _env = env;
            this._userManager = _userManager;
            _config = config;
        }
        public async Task<List<BarDto>> GetBarsAsync()
        {
            var bars = await _context.Bars.Select(x => new BarDto
            {
                Title = x.Title,
                Id = x.Id,
                hasChild = x.hasChild,
                MenusDto = x.Menus.Select(menu => new MenuDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    HasChild = menu.HasChild,
                    ParentId = menu.ParentId,
                    BarId = menu.BarId,
                }).ToList()

            }).ToListAsync();


            return bars;
        }
        public async Task<BarDto> GetBarAsyncById(int id)
        {
            var bar = await _context.Bars
                .Where(b => b.Id == id)
                .Select(b => new BarDto
                {
                    Title = b.Title,
                    Id = b.Id,
                    hasChild = b.hasChild,
                    MenusDto = b.Menus
                        .Where(m => m.BarId == b.Id) // Filter menus based on BarId
                        .Select(menu => new MenuDto
                        {
                            Id = menu.Id,
                            Name = menu.Name,
                            HasChild = menu.HasChild,
                            ParentId = menu.ParentId,
                            BarId = menu.BarId,
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return bar;
        }
        public async Task<bool> CheckClientAsync(string codeClient)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.codeClient == codeClient);
            return client != null;
        }
        /*
        public async Task CreerClient(CreationClient client)
        {
            try
            {
                Console.WriteLine("######client " );

                //Domicile domicile = await _context.Domiciles.FirstOrDefaultAsync(x => x.codeClient == client.codeClient);
                
                //Console.WriteLine("######client "+ domicile.codeClient+' '+ domicile.ville);

                var client1 = new Client
                {
                    codeClient = client.codeClient,
                    nom = client.nom,
                    prenom = client.prenom,
                    dateDelivrance = client.dateDelivrance,
                    dateNaissance = client.dateNaissance,
                    dateRelation = client.dateRelation,
                    lieuNaissance = client.lieuNaissance,
                    civilite = client.civilite,
                    sitFamiliale = client.sitFamiliale,
                    nombreEnfants = client.nombreEnfants,
                    cin = client.cin,
                    codeImputation = client.codeImputation,
                    telephone = client.telephone,
                    statutOccupationLogement = client.statutOccupationLogement,
                };

                //if (domicile != null)
                //{
                //    client1.domicile = domicile;
                //    client1.domicileId = domicile.Id;
                //    domicile.client = client1;
                //
                //}

                _context.Clients.Add(client1);

                await _context.SaveChangesAsync(); // Save changes asynchronously
            }
            catch(Exception ex)
            {
                Console.WriteLine("###### " + ex.ToString());
            }

        }

        public async Task CreerDomicile(DomicileDto domicile)
        {
            try
            {
                Console.WriteLine("######## isnide domicile");

                Client client = await _context.Clients.FirstOrDefaultAsync(x => x.codeClient == domicile.codeClient);

                var domicile1 = new Domicile
                {
                    adresse = domicile.adresse,
                    ville = domicile.ville,
                    codePostal = domicile.codePostal,
                    codeClient = client?.codeClient,
                    client = client,

                };

                _context.Domiciles.Add(domicile1);
                await _context.SaveChangesAsync();

                Domicile dom = await _context.Domiciles.FirstOrDefaultAsync(x=>x.codeClient == domicile1.codeClient);

                if (client != null)
                {
                    client.domicileId=dom.Id;
                    client.domicile=dom;
                }
                await _context.SaveChangesAsync(); // Save changes asynchronously}
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
            
        }*/
        public async Task<List<CreationClient>> GetAllClients()
        {
            {
                var clients = await _context.Clients.Select(x => new CreationClient
                {
                    codeClient = x.codeClient,
                    nom = x.nom,
                    prenom = x.prenom,
                    dateRelation = x.dateRelation,
                    dateDelivrance = x.dateDelivrance,
                    dateNaissance = x.dateNaissance,
                    lieuNaissance = x.lieuNaissance,
                    civilite = x.civilite,
                    sitFamiliale = x.sitFamiliale,
                    nombreEnfants = x.nombreEnfants,
                    cin = x.cin,
                    codeImputation = x.codeImputation,
                    telephone = x.telephone,
                    ville = x.domicile.des_ville,
                    codePostal = x.domicile.codePostal,
                    adresse = x.domicile.adresse,

                }).ToListAsync();

                return clients;
            }
        }

        public async Task<List<villeDto>> GetAllVilles()
        {
            {
                var villes = await _context.Villes.Select(x => new villeDto
                {
                    name = x.name,

                }).ToListAsync();

                return villes;
            }
        }
        public async Task<List<StatutOccupationDto>> getAllStatutOccupation()
        {
            {
                var statut = await _context.statut.Select(x => new StatutOccupationDto
                {
                    name = x.name,

                }).ToListAsync();

                return statut;
            }
        }
        public async Task CreerClient1(CreationClient clientForm)
        {

            var ville = await _context.Villes.FirstOrDefaultAsync(v => v.name == clientForm.ville);
            var statut = await _context.statut.FirstOrDefaultAsync(s => s.name == clientForm.statutOccupationLogement);

            var domicile1 = new Domicile
            {
                adresse = clientForm.adresse,
                des_ville = clientForm.ville,
                codePostal = clientForm.codePostal,
                ville = ville,
                VilleId = ville.Id
            };
            _context.Domiciles.Add(domicile1);

            var client1 = new Client
            {
                codeClient = clientForm.codeClient,
                nom = clientForm.nom,
                prenom = clientForm.prenom,
                dateDelivrance = clientForm.dateDelivrance,
                dateNaissance = clientForm.dateNaissance,
                dateRelation = clientForm.dateRelation,
                lieuNaissance = clientForm.lieuNaissance,
                civilite = clientForm.civilite,
                sitFamiliale = clientForm.sitFamiliale,
                nombreEnfants = clientForm.nombreEnfants,
                cin = clientForm.cin,
                codeImputation = clientForm.codeImputation,
                telephone = clientForm.telephone,
                //statutOccupationLogement = clientForm.statutOccupationLogement,
                statutOccupationLogement = statut,
                StatutOccupationLogementId=statut.Id,
                statut_des= statut.name,
                domicile = domicile1,
                domicileId = domicile1.Id,

            };
            _context.Clients.Add(client1);
            _context.SaveChanges();

        }

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
        public async Task<bool> CheckDossierAsync(string reference)
        {
            var dossier = await _context.Dossiers.FirstOrDefaultAsync(d => d.reference == reference);
            return dossier != null;
        }
        public async Task<bool> creerDossier(DossierDto dossier,string userId)
        {
            try
            {
                Console.WriteLine("#### Inside CreerDossier");
                Client client1 = await _context.Clients.FirstOrDefaultAsync(c => c.codeClient == dossier.codeClient);
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id==userId);


                if (client1 == null)
                {
                    Console.WriteLine("#### Client not found");
                    return false;
                }

                Console.WriteLine("#### Client found: " + dossier.codeClient);

                var dossier1 = new Dossier
                {
                    dateOp = dossier.dateOp,
                    premiereEcheance = dossier.premiereEcheance,
                    derniereEcheance = dossier.derniereEcheance,
                    echeance = dossier.echeance,
                    assurance = dossier.assurance,
                    credit = dossier.credit,
                    periodicite = dossier.periodicite,
                    duree = dossier.duree,
                    codeComptable = dossier.codeComptable,
                    differe = dossier.differe,
                    reference = dossier.reference,
                    produit = dossier.produit,
                    agence = dossier.agence,
                    client = client1,
                    ClientId = client1.Id,
                    UserId= user.Id,
                    user=user,
                };

                if (dossier.cession != null)
                {
                    var fileName = await UploadFile(dossier.cession, @"dossiers\");
                    if (string.IsNullOrEmpty(fileName))
                    {
                        Console.WriteLine("############## Failed to upload cession file.");
                        return false;
                    }

                    dossier1.cession = fileName;
                }

                Console.WriteLine("#### File successfully uploaded");
                _context.Dossiers.Add(dossier1);

                client1.dossiers.Add(dossier1);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("#### Exception: " + ex.Message);
                return false;
            }
        }

        public async Task<CreationClient> getClientBycode(string codeClient)
        {
            var client = _context.Clients.FirstOrDefault(c => c.codeClient == codeClient);
            var domicile = _context.Domiciles.FirstOrDefault(d => d.Id == client.domicileId);
            var statut = _context.statut.FirstOrDefault(s => s.Id == client.StatutOccupationLogementId);
            var ville = _context.Villes.FirstOrDefault(v => v.Id == domicile.VilleId);
            CreationClient creationClient = new CreationClient
            {
                codeClient = client.codeClient,
                nom = client.nom,
                prenom = client.prenom,
                dateDelivrance = client.dateDelivrance,
                dateNaissance = client.dateNaissance,
                dateRelation = client.dateRelation,
                lieuNaissance = client.lieuNaissance,
                civilite = client.civilite,
                sitFamiliale = client.sitFamiliale,
                nombreEnfants = client.nombreEnfants,
                cin = client.cin,
                codeImputation = client.codeImputation,
                telephone = client.telephone,
                statutOccupationLogement = statut.name,
                ville = domicile.des_ville,
                adresse = domicile.adresse,
                codePostal = domicile.codePostal,
            };
            return creationClient;
        }

        public async Task<bool> updateClient(CreationClient data)
        {
            try
            {
                Client client = await _context.Clients.FirstOrDefaultAsync(c => c.codeClient == data.codeClient);
                Domicile domicile = await _context.Domiciles.FirstOrDefaultAsync(d => d.Id == client.domicileId);
                ville ville = await _context.Villes.FirstOrDefaultAsync(v => v.name == data.ville);
                StatutOccupationLogement statut = await _context.statut.FirstOrDefaultAsync(s => s.name == data.statutOccupationLogement);

                client.telephone = data.telephone;
                client.prenom = data.prenom;
                client.nom = data.nom;
                client.dateNaissance = data.dateNaissance;
                client.dateDelivrance = data.dateDelivrance;
                client.cin = data.cin;
                client.civilite = data.civilite;
                client.codeClient = data.codeClient;
                client.dateRelation = data.dateRelation;
                client.lieuNaissance = data.lieuNaissance;
                client.sitFamiliale = data.sitFamiliale;
                client.nombreEnfants = data.nombreEnfants;

                client.statutOccupationLogement = statut;
                client.StatutOccupationLogementId = statut.Id;
                client.statut_des = statut.name;
                _context.SaveChanges();

                domicile.adresse = data.adresse;
                domicile.des_ville = data.ville;
                domicile.codePostal = data.codePostal;
                domicile.ville = ville;
                domicile.VilleId = ville.Id;

                _context.SaveChanges();

                client.domicile = domicile;
                client.domicileId = domicile.Id;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<CreationClient>> getClientByLabel(string label)
        {

            Console.WriteLine("##### inside se");

            var clients = await _context.Clients
                .Where(c =>
                    c.codeClient.Contains(label) ||
                    c.nom.Contains(label) ||
                    c.prenom.Contains(label) ||
                    c.civilite.Contains(label) ||
                    c.sitFamiliale.Contains(label) ||
                    c.cin.Contains(label) ||
                    c.codeImputation.Contains(label) ||
                    c.telephone.Contains(label) ||
                    //c.statutOccupationLogement.Contains(label) ||
                    c.domicile.des_ville.Contains(label) ||
                    c.domicile.adresse.Contains(label) ||
                    c.domicile.codePostal.Contains(label)
                    )
                .Select(c => new CreationClient
                {
                    codeClient = c.codeClient,
                    nom = c.nom,
                    prenom = c.prenom,
                    dateRelation = c.dateRelation,
                    dateNaissance = c.dateNaissance,
                    lieuNaissance = c.lieuNaissance,
                    civilite=c.civilite,
                    sitFamiliale=c.sitFamiliale,
                    nombreEnfants=c.nombreEnfants,
                    cin=c.cin,
                    dateDelivrance=c.dateDelivrance,
                    codeImputation=c.codeImputation,
                    telephone=c.telephone,
                    //statutOccupationLogement=c.statutOccupationLogement,
                    ville=c.domicile.des_ville,
                    codePostal=c.domicile.codePostal,
                    adresse= c.domicile.adresse,
                })
                .ToListAsync();
            if (clients != null && clients.Count > 0)
            {
                foreach (var client in clients)
                {
                    Console.WriteLine("####### " + client.codeClient);
                }
                return clients;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<DossierDto>> getAllDossiers()
        {
            {
                var dossiers = await _context.Dossiers.Select(x => new DossierDto
                {
                    dateOp= x.dateOp,
                    premiereEcheance=x.premiereEcheance,
                    derniereEcheance=x.derniereEcheance,
                    echeance=x.echeance,
                    credit=x.credit,
                    codeClient=x.client.codeClient,
                    periodicite=x.periodicite,
                    duree=x.duree,
                    agence=x.agence,
                    reference=x.reference,
                    produit=x.produit,
                    differe=x.differe,
                    assurance=x.assurance,
                    codeComptable=x.codeComptable,
                    cessionPath=x.cession,

                }).ToListAsync();

                return dossiers;
            }
        }

        public async Task<ActionResult<List<DossierDto>>> getDossiersByLabel(string label)
        {
            var dossiers = await _context.Dossiers
                            .Where(c =>
                                c.credit.Contains(label) ||
                                c.periodicite.Contains(label) ||
                                c.duree.Contains(label) ||
                                c.reference.Contains(label) ||
                                c.produit.Contains(label) ||
                                c.agence.Contains(label) ||
                                c.differe.Contains(label) ||
                                c.codeComptable.Contains(label) ||
                                c.client.codeClient.Contains(label)
                                ).Select(c => new DossierDto
                                {
                                    dateOp = c.dateOp,
                                    premiereEcheance = c.premiereEcheance,
                                    derniereEcheance = c.derniereEcheance,
                                    echeance = c.echeance,
                                    credit = c.credit,
                                    periodicite = c.periodicite,
                                    duree = c.duree,
                                    reference = c.reference,
                                    produit = c.produit,
                                    agence = c.agence,
                                    differe = c.differe,
                                    assurance = c.assurance,
                                    codeComptable = c.codeComptable,
                                    codeClient = c.client.codeClient ,
                                    userId = c.UserId,
                                }).ToListAsync();
            if (dossiers != null && dossiers.Count > 0)
            {
                foreach (var dossier in dossiers)
                {
                    Console.WriteLine("####### " + dossier.reference);
                }
                return dossiers;
            }
            else
            {
                return null;
            }
        }

        private async Task<string> GetFileAsync(string path)
        {
            try
            {
                Console.WriteLine("## " + path);

                var filepath = Path.Combine(this._env.WebRootPath, path);
                Console.WriteLine("## exist  " + filepath);

                if (System.IO.File.Exists(filepath))
                {
                    Console.WriteLine("## exist  " + filepath);

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    string base64ImageString = "data:text/plain/;base64," + Convert.ToBase64String(fileBytes);
                    return base64ImageString;
                }

                // Handle case where file doesn't exist
                Console.WriteLine("## File does not exist: " + filepath);
                return null;
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error retrieving file: {ex.Message}");
                return null;
            }
        }

        public async Task<DossierDto> getDossierByReference(string reference)
        {
            var d = await _context.Dossiers.FirstOrDefaultAsync(d => d.reference == reference);
            var c = await _context.Clients.FirstOrDefaultAsync(c => c.Id == d.ClientId);
            ClientDto client = new ClientDto
            {
                codeClient = c.codeClient,
                nom = c.nom,
                prenom = c.prenom,
            };
            DossierDto dossier = new DossierDto
            {
                dateOp = d.dateOp,
                premiereEcheance = d.premiereEcheance,
                derniereEcheance = d.derniereEcheance,
                echeance = d.echeance,
                credit = d.credit,
                periodicite = d.periodicite,
                duree = d.duree,
                reference = d.reference,
                produit = d.produit,
                agence = d.agence,
                differe = d.differe,
                assurance = d.assurance,
                codeComptable = d.codeComptable,
                codeClient = c.codeClient,
                cessionPath =d.cession,
                clientDto =client,
                userId = d.UserId,
                cessionByte = await this.GetFileAsync(($"dossiers/{d.cession}"))
            };
            return dossier;
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                string relativePath = Path.Combine("dossiers", fileName);
                string filePath = Path.Combine(_env.WebRootPath, relativePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> updateDossier(DossierDto dossier)
        {
            Dossier dossierAnc = await _context.Dossiers.FirstOrDefaultAsync(d => d.reference == dossier.referenceAnc);


            dossierAnc.dateOp = dossier.dateOp;
            dossierAnc.premiereEcheance = dossier.premiereEcheance;
            dossierAnc.derniereEcheance = dossier.derniereEcheance;
            dossierAnc.echeance = dossier.echeance;
            dossierAnc.assurance = dossier.assurance;
            dossierAnc.credit = dossier.credit;
            dossierAnc.periodicite = dossier.periodicite;
            dossierAnc.duree = dossier.duree;
            dossierAnc.codeComptable = dossier.codeComptable;
            dossierAnc.differe = dossier.differe;
            dossierAnc.reference = dossier.reference;
            dossierAnc.produit = dossier.produit;
            dossierAnc.agence = dossier.agence;


            if (!string.IsNullOrEmpty(dossierAnc.cession))
            {
                DeleteFile(dossierAnc.cession);
            }

            dossierAnc.cession = null;

            Console.WriteLine(dossier.cessionByte + ' ' + dossier.cessionPath);

            if (dossier.cession!=null)
            {
                Console.WriteLine("'############'" + dossier.cessionPath);
                dossierAnc.cession = dossier.cessionPath;
                UploadFile(dossier.cession, @"dossiers\");
            }
            _context.Dossiers.Update(dossierAnc);
            var result = await _context.SaveChangesAsync();
            return result > 0;


        }


    }

}
