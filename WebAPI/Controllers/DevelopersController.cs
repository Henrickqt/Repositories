using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.WebAPI.Contexts;
using Repositories.WebAPI.DTO;
using Repositories.WebAPI.Models;
using Repositories.WebAPI.Utils;

namespace Repositories.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevelopersController : ControllerBase
    {
        private readonly RepositoriesContext _context;

        public DevelopersController(RepositoriesContext context)
        {
            _context = context;
        }

        /// <summary>Recupera todos os Desenvolvedores</summary>
        /// <returns>Retorna uma lista com todos os Desenvolvedores</returns>
        /// <response code="200">Se a lista com todos os Desenvolvedores foi retornada</response>
        // GET: api/Developers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeveloperResDTO>>> GetDeveloper()
        {
            return await _context.Developer
                .OrderBy(developer => developer.Name)
                .Select(developer => ToDeveloperResDTO(developer))
                .ToListAsync();
        }

        /// <summary>Recupera um Desenvolvedor específico</summary>
        /// <param name="email">O email do Desenvolvedor</param>
        /// <returns>Retorna o Desenvolvedor requisitado</returns>
        /// <response code="200">Se o Desenvolvedor requisitado foi retornado</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        // GET: api/Developers/email@example.com
        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeveloperResDTO>> GetDeveloper(string email)
        {
            var developer = await _context.Developer.FindAsync(email);

            if (developer == null)
            {
                return NotFound();
            }

            return ToDeveloperResDTO(developer);
        }

        /// <summary>Atualiza os dados de um Desenvolvedor</summary>
        /// <param name="email">O email do Desenvolvedor</param>
        /// <param name="developer">Os novos dados do Desenvolvedor</param>
        /// <response code="204">Se o Desenvolvedor requisitado foi atualizado</response>
        /// <response code="400">Se a requisição está com a sintaxe errada</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        /// <response code="500">Se ocorreu algum erro durante a atualização</response>
        // PUT: api/Developers/email@example.com
        [HttpPut("{email}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutDeveloper(string email, DeveloperReqDTO developer)
        {
            string errors = string.Empty;

            errors += (email != developer.Email) ? Validators.ERROR_CHANGING_EMAIL_MSG : "";
            errors += (!Validators.IsValidName(developer.Name)) ? Validators.INVALID_NAME_MSG : "";
            errors += (!Validators.IsValidBio(developer.Bio)) ? Validators.INVALID_BIO_MSG : "";
            errors += (!Validators.IsValidPassword(developer.Password)) ? Validators.INVALID_PASSWORD_MSG : "";

            if (errors.Length > 0)
            {
                return BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "Bad Request",
                    status = 400,
                    message = errors
                });
            }

            var developerData = await _context.Developer.FindAsync(email);

            if (developerData == null)
            {
                return NotFound();
            }

            developerData.Name = developer.Name;
            developerData.Bio = developer.Bio;
            developerData.Password = developer.Password;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperExists(email))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>Armazena um novo Desenvolvedor</summary>
        /// <param name="developer">Os dados do Desenvolvedor</param>
        /// <returns>Retorna o Desenvolvedor recém criado</returns>
        /// <response code="201">Se o Desenvolvedor foi armazenado</response>
        /// <response code="400">Se a requisição está com a sintaxe errada</response>
        /// <response code="409">Se já existe um Desenvolvedor com o mesmo email</response>
        /// <response code="500">Se ocorreu algum erro durante o armazenamento</response>
        // POST: api/Developers
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeveloperResDTO>> PostDeveloper(DeveloperReqDTO developer)
        {
            string errors = string.Empty;

            errors += (!Validators.IsValidName(developer.Name)) ? Validators.INVALID_NAME_MSG : "";
            errors += (!Validators.IsValidBio(developer.Bio)) ? Validators.INVALID_BIO_MSG : "";
            errors += (!Validators.IsValidEmail(developer.Email)) ? Validators.INVALID_EMAIL_MSG : "";
            errors += (!Validators.IsValidPassword(developer.Password)) ? Validators.INVALID_PASSWORD_MSG : "";

            if (errors.Length > 0)
            {
                return BadRequest(new 
                { 
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1", 
                    title = "Bad Request", 
                    status = 400, 
                    message = errors 
                });
            }

            Developer developerNew = new Developer
            {
                Name = developer.Name,
                Bio = developer.Bio,
                Email = developer.Email,
                Password = developer.Password,
                Project = new List<Project>()
            };

            _context.Developer.Add(developerNew);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeveloperExists(developer.Email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDeveloper", new { id = developer.Email }, ToDeveloperResDTO(developerNew));
        }

        /// <summary>Deleta um Desenvolvedor específico e seus Projetos</summary>
        /// <param name="email">O email do Desenvolvedor</param>
        /// <returns>Retorna o Desenvolvedor deletado</returns>
        /// <response code="200">Se o Desenvolvedor requisitado foi deletado</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        // DELETE: api/Developers/email@example.com
        [HttpDelete("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeveloperResDTO>> DeleteDeveloper(string email)
        {
            var developer = await _context.Developer.FindAsync(email);

            if (developer == null)
            {
                return NotFound();
            }

            _context.Developer.Remove(developer);
            await _context.SaveChangesAsync();

            return ToDeveloperResDTO(developer);
        }

        private bool DeveloperExists(string email)
        {
            return _context.Developer.Any(developer => developer.Email == email);
        }

        private static DeveloperResDTO ToDeveloperResDTO(Developer developer) =>
            new DeveloperResDTO
            {
                Name = developer.Name,
                Bio = developer.Bio,
                Email = developer.Email
            };
    }
}
