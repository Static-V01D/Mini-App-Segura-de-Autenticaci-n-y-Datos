namespace LibraryApp.Models;

public class Request : IEquatable<Request>
{
    private Models.User user;
    private Models.Book book;
    public Request(Models.User user, Models.Book book)
    {
        SetBook(book);
        SetUser(user);
    }

    public void SetUser(Models.User u)
    {
        if (u is null) throw new ArgumentNullException(nameof(u));
        else user = u;
    }
    public void SetBook(Models.Book b)
    {
        if (b is null) throw new ArgumentNullException(nameof(b));
        else book = b;
    }

    public Models.Book GetBook() { return book; }
    public Models.User GetUser() { return user; }

    public bool Equals(Request? other)
    {
        return Equals((object?)other);
    }
    public override bool Equals(object? obj)
    {
        return obj is Request other &&
        (ReferenceEquals(this, other) ||
        user == other.user && book == other.book);
    }
    public static bool operator ==(Request book1, Request book2)
    {
        return book1 is null ? book2 is null : book1.Equals(book2);
    }
    public static bool operator !=(Request book1, Request book2)
    {
        return !(book1 == book2);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(user, book);
    }
    public override string ToString()
    {
        return $"{GetType().Name}: {user},\n {book}";
    }
}

