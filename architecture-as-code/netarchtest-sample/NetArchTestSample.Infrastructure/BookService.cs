using Microsoft.Extensions.Logging;
using NetArchTestSample.Domain.Entities;
using NetArchTestSample.Domain.Interfaces;

namespace NetArchTestSample.Infrastructure.Services;

public sealed class BookService(IUnitOfWork unitOfWork, ILogger<BookService> logger) : IBookService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<BookService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Book> CreateBookAsync(string title, string author, string isbn, DateTime publishedDate, int pageCount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new book with title: {Title}", title);
        
        var book = new Book(title, author, isbn, publishedDate, pageCount);
        
        await _unitOfWork.Books.AddAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully created book with ID: {BookId}", book.Id);
        return book;
    }

    public async Task<Book?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving book with ID: {BookId}", id);
        return await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all books");
        return await _unitOfWork.Books.GetAllAsync(cancellationToken);
    }

    public async Task<Book> UpdateBookAsync(Guid id, string title, string author, int pageCount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating book with ID: {BookId}", id);
        
        var book = await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            _logger.LogWarning("Attempted to update non-existent book with ID: {BookId}", id);
            throw new InvalidOperationException($"Book with ID {id} not found");
        }

        book.UpdateDetails(title, author, pageCount);
        
        await _unitOfWork.Books.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully updated book with ID: {BookId}", id);
        return book;
    }

    public async Task DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting book with ID: {BookId}", id);
        
        var book = await _unitOfWork.Books.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            _logger.LogWarning("Attempted to delete non-existent book with ID: {BookId}", id);
            throw new InvalidOperationException($"Book with ID {id} not found");
        }

        await _unitOfWork.Books.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully deleted book with ID: {BookId}", id);
    }
}
