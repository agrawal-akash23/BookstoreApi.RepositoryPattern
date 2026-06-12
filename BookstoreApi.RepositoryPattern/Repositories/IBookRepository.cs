using BookstoreApi.RepositoryPattern.Models;

namespace BookstoreApi.RepositoryPattern.Repositories
{
    // An interface is a contract - it says "any class that implements me
    // MUST provide all of these methods." No implementation code here.
    // Just method signatures (name, parameters, return type).
    public interface IBookRepository
    {
        // Returns all books. Task = async. List<Book> = the result type.
        Task<List<Book>> GetAllAsync();

        // Returns one book by ID, or null if it doesn't exist
        Task<Book?> GetByIdAsync(int id);

        // Returns books whose title contains the search term
        Task<List<Book>> SearchByTitleAsync(string title);

        // Returns books under a given price - the raw SQL / stored proc endpoint
        Task<List<Book>> GetByPriceRangeAsync(decimal min, decimal max);

        // Creates a new book and returns it with its database-assigned ID set
        Task<Book> CreateAsync(Book book);

        // Updates a book. Returns false if the book wasn't found.
        Task<bool> UpdateAsync(int id, Book updated);

        // Deletes a book. Returns false if the book wasn't found.
        Task<bool> DeleteAsync(int id);
    }
}
