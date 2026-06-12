using BookstoreApi.RepositoryPattern.Models;
using BookstoreApi.RepositoryPattern.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.RepositoryPattern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // resolves to api/books
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repo;

        // No more DbContext - controller only knows about the interface
        public BooksController(IBookRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAll()
        {
            return Ok(await _repo.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(int id)
        {
            var book = await _repo.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Book>>> Search([FromQuery] string title) =>
            Ok(await _repo.SearchByTitleAsync(title));

        [HttpPost]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            var created = _repo.CreateAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Book updated)
        {
            var success = await _repo.UpdateAsync(id, updated);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repo.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<List<Book>>> GetByPriceRange(
            [FromQuery] decimal min = 0,
            [FromQuery] decimal max = 100)
        {
            // Input validation — always validate parameters before using them in SQL
            if (min < 0 || max < 0)
                return BadRequest(new { message = "Price cannot be negative." });

            if (min > max)
                return BadRequest(new { message = $"min ({min}) cannot exceed max ({max})." });

            return Ok(await _repo.GetByPriceRangeAsync(min, max));
        }
    }
}
