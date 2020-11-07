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
    public class ProjectsController : ControllerBase
    {
        private readonly RepositoriesContext _context;

        public ProjectsController(RepositoriesContext context)
        {
            _context = context;
        }

        /// <summary>Recupera todos os Projetos</summary>
        /// <returns>Retorna uma lista com todos os Projetos</returns>
        /// <response code="200">Se a lista com todos os Projetos foi retornada</response>
        // GET: api/Projects
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProject()
        {
            return await _context.Project
                .OrderBy(project => project.Name)
                .Select(project => ToProjectDTO(project))
                .ToListAsync();
        }

        /// <summary>Recupera um Projeto específico</summary>
        /// <param name="id">O id do Projeto</param>
        /// <returns>Retorna o Projeto requisitado</returns>
        /// <response code="200">Se o Projeto requisitado foi retornado</response>
        /// <response code="404">Se o Projeto não foi encontrado</response>
        // GET: api/Projects/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDTO>> GetProject(int id)
        {
            var project = await _context.Project.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return ToProjectDTO(project);
        }

        /// <summary>Recupera todos os Projetos de um Desenvolvedor</summary>
        /// <param name="email">O email do Desenvolvedor</param>
        /// <returns>Retorna uma lista com todos os Projetos</returns>
        /// <response code="200">Se a lista com todos os Projetos foi retornada</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        // GET: api/Projects/Developers/email@example.com
        [HttpGet("Developers/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProject(string email)
        {
            var developer = await _context.Developer.FindAsync(email);

            if (developer == null)
            {
                return NotFound();
            }

            return await _context.Project
                .Where(project => project.IdDeveloper == email)
                .OrderBy(project => project.Name)
                .Select(project => ToProjectDTO(project))
                .ToListAsync();
        }

        /// <summary>Atualiza os dados de um Projeto</summary>
        /// <param name="id">O id do Projeto</param>
        /// <param name="project">Os novos dados do Projeto</param>
        /// <response code="204">Se o Projeto requisitado foi atualizado</response>
        /// <response code="400">Se a requisição está com a sintaxe errada</response>
        /// <response code="404">Se o Projeto não foi encontrado</response>
        /// <response code="500">Se ocorreu algum erro durante a atualização</response>
        // PUT: api/Projects/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutProject(int id, ProjectDTO project)
        {
            string errors = string.Empty;

            errors += (id != project.Id) ? Validators.ERROR_CHANGING_ID_MSG : "";
            errors += (!Validators.IsValidProjectName(project.Name)) ? Validators.INVALID_PROJECT_NAME_MSG : "";
            errors += (!Validators.IsValidDescription(project.Description)) ? Validators.INVALID_DESCRIPTION_MSG : "";
            errors += (!Validators.IsValidLanguages(project.Languages)) ? Validators.INVALID_LANGUAGES_MSG : "";

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

            var projectData = await _context.Project.FindAsync(id);

            if (projectData == null)
            {
                return NotFound();
            }

            projectData.Name = project.Name;
            projectData.Description = project.Description;
            projectData.Languages = project.Languages;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        /// <summary>Armazena um novo Projeto</summary>
        /// <param name="project">Os dados do Projeto</param>
        /// <returns>Retorna o Projeto recém criado</returns>
        /// <response code="201">Se o Projeto foi armazenado</response>
        /// <response code="400">Se a requisição está com a sintaxe errada</response>
        /// <response code="404">Se o Desenvolvedor não foi encontrado</response>
        /// <response code="500">Se ocorreu algum erro durante o armazenamento</response>
        // POST: api/Projects
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO project)
        {
            string errors = string.Empty;

            errors += (!Validators.IsValidProjectName(project.Name)) ? Validators.INVALID_PROJECT_NAME_MSG : "";
            errors += (!Validators.IsValidDescription(project.Description)) ? Validators.INVALID_DESCRIPTION_MSG : "";
            errors += (!Validators.IsValidLanguages(project.Languages)) ? Validators.INVALID_LANGUAGES_MSG : "";

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

            var developer = await _context.Developer.FindAsync(project.IdDeveloper);

            if (developer == null)
            {
                return NotFound();
            }

            Project projectNew = new Project
            {
                IdDeveloper = project.IdDeveloper,
                Name = project.Name,
                Description = project.Description,
                Languages = project.Languages
            };

            _context.Project.Add(projectNew);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = projectNew.Id }, ToProjectDTO(projectNew));
        }

        /// <summary>Deleta um Projeto específico</summary>
        /// <param name="id">O id do Projeto</param>
        /// <returns>Retorna o Projeto deletado</returns>
        /// <response code="200">Se o Projeto requisitado foi deletado</response>
        /// <response code="404">Se o Projeto não foi encontrado</response>
        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDTO>> DeleteProject(int id)
        {
            var project = await _context.Project.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return ToProjectDTO(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(project => project.Id == id);
        }

        private static ProjectDTO ToProjectDTO(Project project) =>
            new ProjectDTO()
            {
                Id = project.Id,
                IdDeveloper = project.IdDeveloper,
                Name = project.Name,
                Description = project.Description,
                Languages = project.Languages
            };
    }
}
