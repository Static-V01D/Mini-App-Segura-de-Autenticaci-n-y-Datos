using System.Text.Json.Serialization;
namespace LibraryApp.Models;

public class LoanedBooks : IEquatable<LoanedBooks>
{
    [JsonInclude] private string title;
    [JsonInclude] private string author;
    [JsonInclude] private DateOnly dueDate;
    [JsonInclude] private DateOnly dateLoaned;

    public LoanedBooks() : this(new Book("Placeholder", "Placeholder"), DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now)) { }
    public LoanedBooks(Book book, DateOnly due, DateOnly currentDay)
    {
        SetTitle(book.GetTitle());
        SetAuthor(book.GetAuthor());
        SetDueDate(due);
        SetDateLoaned(currentDay);
    }
    public void SetTitle(string t)
    {
        if (t is null) throw new ArgumentException(null, nameof(t));

        else title = t.ToUpper();
    }
    public void SetAuthor(string a)
    {
        if (a is null) throw new ArgumentException(null, nameof(a));

        else author = a.ToUpper();
    }
    public void SetDueDate(DateOnly due)
    {
        if (due == DateOnly.MinValue) throw new ArgumentException(null, nameof(due));

        else dueDate = due;

    }
    public void SetDateLoaned(DateOnly date)
    {
        if (date == DateOnly.MinValue) throw new ArgumentException(null, nameof(date));

        else dateLoaned = date;

    }
    public string GetTitle() { return title; }
    public string GetAuthor() { return author; }
    public DateOnly GetDueDate() { return dueDate; }
    public DateOnly GetDateLoaned() { return dateLoaned; }

    public override string ToString()
    {
        return $"{GetType().Name}: Title= {title}, Author= {author}, DateLoaned= {dateLoaned}, DueDate= {dueDate}";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(title, author);
    }

    public bool Equals(LoanedBooks? other)
    {
        return Equals((object?)other);
    }
    public override bool Equals(object? obj)
    {
        return obj is LoanedBooks other &&
        (ReferenceEquals(this, other) ||
        title == other.title && author == other.author);
    }
    public static bool operator ==(LoanedBooks book1, LoanedBooks book2)
    {
        return book1 is null ? book2 is null : book1.Equals(book2);
    }
    public static bool operator !=(LoanedBooks book1, LoanedBooks book2)
    {
        return !(book1 == book2);
    }
}