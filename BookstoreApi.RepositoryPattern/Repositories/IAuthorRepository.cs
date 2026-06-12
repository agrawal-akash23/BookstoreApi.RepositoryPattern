using BookstoreApi.RepositoryPattern.Models;

namespace BookstoreApi.RepositoryPattern.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAsync();
        Task<Author?> GetByIdAsync(int id);
        Task<bool> ExistsWithNameAsync(string name);    // for duplicate check
        Task<Author> CreateAsync(Author author);
        Task<bool> UpdateAsync(int id, Author author);
        Task<bool> DeleteAsync(int id);
    }
}
