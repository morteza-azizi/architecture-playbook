using Microsoft.AspNetCore.Mvc;
using NetArchTestSample.Domain.Entities;
using NetArchTestSample.Domain.Interfaces;

namespace NetArchTestSample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all books
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all books</returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<Book>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all books");
        var books = await _bookService.GetAllBooksAsync(cancellationToken);
        return Ok(books);
    }

    /// <summary>
    /// Get a specific book by ID
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested book</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<Book>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> GetBook(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting book with ID: {BookId}", id);
        var book = await _bookService.GetBookByIdAsync(id, cancellationToken);
        
        if (book == null)
        {
            _logger.LogWarning("Book with ID {BookId} not found", id);
            return NotFound($"Book with ID {id} not found");
        }

        return Ok(book);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    /// <param name="request">Book creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created book</returns>
    [HttpPost]
    [ProducesResponseType<Book>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Book>> CreateBook([FromBody] CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new book: {Title}", request.Title);
        
        try
        {
            var book = await _bookService.CreateBookAsync(
                request.Title, 
                request.Author, 
                request.ISBN, 
                request.PublishedDate, 
                request.PageCount, 
                cancellationToken);

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create book: {Message}", ex.Message);
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid book data: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing book
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <param name="request">Book update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated book</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<Book>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> UpdateBook(Guid id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating book with ID: {BookId}", id);
        
        try
        {
            var book = await _bookService.UpdateBookAsync(id, request.Title, request.Author, request.PageCount, cancellationToken);
            return Ok(book);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Book not found: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid book data: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a book
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting book with ID: {BookId}", id);
        
        try
        {
            await _bookService.DeleteBookAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Book not found: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
    }
}

public sealed record CreateBookRequest(
    string Title,
    string Author,
    string ISBN,
    DateTime PublishedDate,
    int PageCount
);

public sealed record UpdateBookRequest(
    string Title,
    string Author,
    int PageCount
); 