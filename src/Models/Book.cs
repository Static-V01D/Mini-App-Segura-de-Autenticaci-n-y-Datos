using DotNetEnv;
using LibraryApp.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryApp.Models;

public class Book : IEquatable<Book>
{
    [JsonInclude] private string title;
    [JsonInclude] private readonly int id;
    [JsonInclude] private string author;
    [JsonInclude] private bool available;

    public Book(string t, string auth, bool av)
    {
        SetAuthor(auth);
        SetTitle(t);
        SetAvailable(av);
        id = GetHashCode();
    }
    public Book(string t, string auth) : this(t, auth, true) { }
    public Book() : this("placeholder", "placeholder") { }

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
    public void SetAvailable(bool a)
    {
        available = a;
    }
    public string GetTitle() { return title; }
    public string GetAuthor() { return author; }
    public int GetId() { return id; }
    public bool IsAvailable() { return available; }

    public bool Equals(Book? other)
    {
        return Equals((object?)other);
    }
    public override bool Equals(object? obj)
    {
        return obj is Book other &&
        (ReferenceEquals(this, other) ||
        title == other.title && author == other.author);
    }
    public static bool operator ==(Book book1, Book book2)
    {
        return book1 is null ? book2 is null : book1.Equals(book2);
    }
    public static bool operator !=(Book book1, Book book2)
    {
        return !(book1 == book2);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(title, author);
    }
    public override string ToString()
    {
        return $"{GetType().Name}: Title= {title}, Author= {author}, Available={available}";
    }
}




