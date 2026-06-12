using BookstoreApi.RepositoryPattern.Data;
using BookstoreApi.RepositoryPattern.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApi.RepositoryPattern.Repositories
{
    public class EfAuthorRepository : IAuthorRepository
    {
        private readonly BookstoreDbContext _db;
        public EfAuthorRepository(BookstoreDbContext db) => _db = db;

        public async Task<List<Author>> GetAllAsync() =>
            await _db.Authors
                .Include(a => a.Books)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public async Task<Author?> GetByIdAsync(int id) =>
            await _db.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

        // Business logic moved from controller to repository
        // Any code path that creates an author can now call this single check
        public async Task<bool> ExistsWithNameAsync(string name) =>
            await _db.Authors.AnyAsync(a => a.Name == name);

        public async Task<Author> CreateAsync(Author author)
        {
            _db.Authors.Add(author);
            await _db.SaveChangesAsync();
            return author;
        }

        public async Task<bool> UpdateAsync(int id, Author updated)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author is null) return false;
            author.Name = updated.Name;
            await _db.SaveChangesAsync();
            return true;
        }

        // DeleteAsync: loads books to enforce business rule, returns false if
        // author has books (caller decides what HTTP status to return)
        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return false;

            // If author has books, refuse the delete by returning false
            // The controller can distinguish "not found" (null) from "has books" (false)
            // via a more detailed result type — see the annotation below
            if (author.Books.Any()) return false;

            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
