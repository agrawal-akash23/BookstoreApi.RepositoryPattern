using BookstoreApi.RepositoryPattern.Data;
using BookstoreApi.RepositoryPattern.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApi.RepositoryPattern.Repositories
{
    // This class IMPLEMENTS the interface.
    // The compiler guarantees every method in IBookRepository exists here.
    // If you add a method to the interface and forget to add it here → compile error.
    public class EfBookRepository : IBookRepository
    {
        private readonly BookstoreDbContext _db;

        // DbContext is injected — same pattern as before, just one level lower
        public EfBookRepository(BookstoreDbContext db) => _db = db;

        public async Task<List<Book>> GetAllAsync() =>
            await _db.Books
                .Include(b => b.Author)
                .OrderBy(b => b.Title)
                .ToListAsync();

        public async Task<Book?> GetByIdAsync(int id) =>
            await _db.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
        // Returns null if not found — matches the Book? return type

        public async Task<List<Book>> SearchByTitleAsync(string title) =>
            await _db.Books
                .Where(b => b.Title.Contains(title))
                .Include(b => b.Author)
                .ToListAsync();

        public async Task<List<Book>> GetByPriceRangeAsync(decimal min, decimal max) =>
            await _db.Books
                .FromSqlRaw(@"
                SELECT b.Id, b.Title, b.Price, b.AuthorId
                FROM Books b
                WHERE b.Price BETWEEN {0} AND {1}
                ORDER BY b.Price ASC", min, max)
                .ToListAsync();

        public async Task<Book> CreateAsync(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();  // sets book.Id after INSERT
            return book;
        }

        public async Task<bool> UpdateAsync(int id, Book updated)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null) return false;   // signal "not found" with false

            book.Title = updated.Title;
            book.Price = updated.Price;
            book.AuthorId = updated.AuthorId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null) return false;

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
