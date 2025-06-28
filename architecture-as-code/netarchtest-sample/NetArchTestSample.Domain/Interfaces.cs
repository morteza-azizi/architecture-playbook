using NetArchTestSample.Domain.Entities;

namespace NetArchTestSample.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IBookService
{
    Task<Book> CreateBookAsync(string title, string author, string isbn, DateTime publishedDate, int pageCount, CancellationToken cancellationToken = default);
    Task<Book?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default);
    Task<Book> UpdateBookAsync(Guid id, string title, string author, int pageCount, CancellationToken cancellationToken = default);
    Task DeleteBookAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 