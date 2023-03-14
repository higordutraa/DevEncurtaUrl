using DevEncurtaURL.API.Entities;
using DevEncurtaURL.API.Models;
using DevEncurtaURL.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DevEncurtaURL.API.Controllers
{
    [ApiController]
    [Route("api/shortenedLinks")]
    public class ShortenedLinksController : ControllerBase
    {
        private readonly DevEncurtaUrlDbContext _context;
        public ShortenedLinksController(DevEncurtaUrlDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter uma listagem dos Ids
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            Log.Information("GetAll is called!");

            return Ok(_context.Links);
        }

        /// <summary>
        /// Obter um Id específico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var link = _context.Links.SingleOrDefault(l => l.Id == id);

            if (link == null) return NotFound();

            return Ok(link);
        }

        /// <summary>
        /// Cadastrar um Link encurtado
        /// </summary>
        /// <remarks>
        /// { "title": "ultimo-repositorio Git", "destinationLink": "https://github.com/higordutraa/DevEncurtaUrl" }
        /// </remarks>
        /// <param name="model">Dados de link</param>
        /// <returns>Objetos recém-criado</returns>
        /// <response code = "201">Sucesso!</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] 
        public IActionResult Post(AddOrUpdateShortenedLinkModel model)
        {
            var link = new ShortenedCustomLink(model.Title, model.DestinationLink);
            
            _context.Links.Add(link);

            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = link.Id }, link);
        }

        /// <summary>
        /// Atualizar um Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id, AddOrUpdateShortenedLinkModel model)
        {
            var link = _context.Links.SingleOrDefault(l => l.Id == id);

            if (link == null) return NotFound();

            link.Update(model.Title, model.DestinationLink);

            _context.Links.Update(link);

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletar um Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var link = _context.Links.SingleOrDefault(l => l.Id == id);

            if (link == null) return NotFound();

            _context.Links.Remove(link);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("/{code}")]
        public IActionResult RedirectLink(string code)
        {
            var link = _context.Links.SingleOrDefault(l => l.Code == code);

            if (link == null) return NotFound();

            return Redirect(link.DestinationLink);
        }
    }
}
