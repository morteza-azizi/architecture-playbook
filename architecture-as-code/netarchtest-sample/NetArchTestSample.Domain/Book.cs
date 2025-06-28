namespace NetArchTestSample.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string ISBN { get; private set; }
    public DateTime PublishedDate { get; private set; }
    public int PageCount { get; private set; }
    public BookStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Book() { } // For EF Core

    public Book(string title, string author, string isbn, DateTime publishedDate, int pageCount)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty", nameof(isbn));
        if (pageCount <= 0)
            throw new ArgumentException("Page count must be positive", nameof(pageCount));

        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        ISBN = isbn;
        PublishedDate = publishedDate;
        PageCount = pageCount;
        Status = BookStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string author, int pageCount)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));
        if (pageCount <= 0)
            throw new ArgumentException("Page count must be positive", nameof(pageCount));

        Title = title;
        Author = author;
        PageCount = pageCount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCheckedOut()
    {
        if (Status != BookStatus.Available)
            throw new InvalidOperationException("Book is not available for checkout");
        
        Status = BookStatus.CheckedOut;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReturned()
    {
        if (Status != BookStatus.CheckedOut)
            throw new InvalidOperationException("Book is not checked out");
        
        Status = BookStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum BookStatus
{
    Available,
    CheckedOut,
    Reserved,
    OutOfService
}
