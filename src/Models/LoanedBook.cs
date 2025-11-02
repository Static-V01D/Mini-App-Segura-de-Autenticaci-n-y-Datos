using System.Text.Json.Serialization;
namespace LibraryApp.Models;

public class LoanedBooks : IEquatable<LoanedBooks>
{
    [JsonInclude] private string title;
    [JsonInclude] private DateOnly dueDate;
    [JsonInclude] private DateOnly dateLoaned;

    public LoanedBooks() : this("PlaceHolder", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now)) { }
    public LoanedBooks(string t, DateOnly due, DateOnly currentDay)
    {
        SetTitle(t);
        SetDueDate(due);
        SetDateLoaned(currentDay);
    }
    public void SetTitle(string t)
    {
        if (t is null) throw new ArgumentException(null, nameof(t));

        else title = t.ToUpper();
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
    public DateOnly GetDueDate() { return dueDate; }
    public DateOnly GetDateLoaned() { return dateLoaned; }

    public override string ToString()
    {
        return $"{GetType().Name}: Title= {title} ,DueDate= {dueDate}";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(title, dueDate, dateLoaned);
    }

    public bool Equals(LoanedBooks? other)
    {
        return Equals((object?)other);
    }
    public override bool Equals(object? obj)
    {
        return obj is LoanedBooks other && // Same type?
        (ReferenceEquals(this, other) || // Same references?
        title == other.title && dueDate == other.dueDate && dateLoaned == other.dateLoaned); // same properties
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

