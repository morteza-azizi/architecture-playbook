using NetArchTestSample.Domain.Entities;
using NetArchTestSample.Domain.Interfaces;
using System.Collections.Concurrent;

namespace NetArchTestSample.Data.Repositories;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly ConcurrentDictionary<Guid, Book> _books = new();

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _books.TryGetValue(id, out var book);
        return Task.FromResult(book);
    }

    public Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<Book>>(_books.Values.ToList());
    }

    public Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        if (_books.TryAdd(book.Id, book))
            return Task.FromResult(book);
        
        throw new InvalidOperationException($"Book with ID {book.Id} already exists");
    }

    public Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        if (_books.ContainsKey(book.Id))
        {
            _books[book.Id] = book;
            return Task.FromResult(book);
        }
        
        throw new InvalidOperationException($"Book with ID {book.Id} not found");
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (!_books.TryRemove(id, out _))
            throw new InvalidOperationException($"Book with ID {id} not found");
        
        return Task.CompletedTask;
    }
}

public sealed class InMemoryUnitOfWork(IBookRepository bookRepository) : IUnitOfWork
{
    public IBookRepository Books { get; } = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would save changes to the database
        // For in-memory implementation, changes are already persisted
        return Task.FromResult(1);
    }
} 