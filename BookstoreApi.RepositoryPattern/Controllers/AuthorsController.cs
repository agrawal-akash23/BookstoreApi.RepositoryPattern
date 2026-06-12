using BookstoreApi.RepositoryPattern.Models;
using BookstoreApi.RepositoryPattern.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.RepositoryPattern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _repo;
        public AuthorsController(IAuthorRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAll() =>
            Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var author = await _repo.GetByIdAsync(id);
            return author is null ? NotFound() : Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<Author>> Create(Author author)
        {
            // Duplicate check via repository method - no LINQ in the controller. 
            if (await _repo.ExistsWithNameAsync(author.Name))
                return Conflict(new { message = $"Author '{author.Name}' already exists." });

            var created = await _repo.CreateAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Author updated)
        {
            var success = await _repo.UpdateAsync(id, updated);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Load first to distinguish "not found" from "has books"
            var author = await _repo.GetByIdAsync(id);
            if (author is null) return NotFound();

            if (author.Books.Any())
                return Conflict(new { message = $"Cannot delete '{author.Name}' - they have {author.Books.Count} book(s). Remove the books first." });

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
