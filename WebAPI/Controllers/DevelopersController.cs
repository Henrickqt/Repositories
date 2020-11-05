using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.WebAPI.Contexts;
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

        /// <summary>Recupera todos os Desenvolvedores com seus Projetos</summary>
        /// <returns>Retorna uma lista com todos os Desenvolvedores</returns>
        /// <response code="200">Se a lista com todos os Desenvolvedores foi retornada</response>
        // GET: api/Developers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Developer>>> GetDeveloper()
        {
            return await _context.Developer.OrderBy(dev => dev.Name).ToListAsync();
        }

        /// <summary>Recupera um Desenvolvedor específico com seus Projetos</summary>
        /// <param name="id">O email do Desenvolvedor</param>
        /// <returns>Retorna o Desenvolvedor requisitado</returns>
        /// <response code="200">Se o Desenvolvedor requisitado foi retornado</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        // GET: api/Developers/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Developer>> GetDeveloper(string id)
        {
            var developer = await _context.Developer.FindAsync(id);

            if (developer == null)
            {
                return NotFound();
            }

            return developer;
        }

        /// <summary>Atualiza os dados de um Desenvolvedor</summary>
        /// <param name="id">O email do Desenvolvedor</param>
        /// <param name="developer">Os novos dados do Desenvolvedor</param>
        /// <response code="204">Se o Desenvolvedor requisitado foi atualizado</response>
        /// <response code="400">Se a requisição está com a sintaxe errada</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        /// <response code="500">Se ocorreu algum erro durante o armazenamento</response>
        // PUT: api/Developers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutDeveloper(string id, Developer developer)
        {
            string errors = string.Empty;

            errors += (id != developer.Email) ? Validators.ERROR_CHANGING_EMAIL_MSG : "";
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

            _context.Entry(developer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperExists(id))
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Developer>> PostDeveloper(Developer developer)
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

            developer.Project = new List<Project>();
            _context.Developer.Add(developer);

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

            return CreatedAtAction("GetDeveloper", new { id = developer.Email }, developer);
        }

        /// <summary>Deleta um Desenvolvedor específico com seus Projetos</summary>
        /// <param name="id">O email do Desenvolvedor</param>
        /// <returns>Retorna o Desenvolvedor deletado</returns>
        /// <response code="200">Se o Desenvolvedor requisitado foi deletado</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        // DELETE: api/Developers/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Developer>> DeleteDeveloper(string id)
        {
            var developer = await _context.Developer.FindAsync(id);
            if (developer == null)
            {
                return NotFound();
            }

            _context.Developer.Remove(developer);
            await _context.SaveChangesAsync();

            return developer;
        }

        private bool DeveloperExists(string id)
        {
            return _context.Developer.Any(e => e.Email == id);
        }
    }
}
