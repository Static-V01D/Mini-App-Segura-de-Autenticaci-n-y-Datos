using LibraryApp.Models;
using System.Text.Json.Serialization;
using DotNetEnv;
using LibraryApp.Services;
using System.Text.Json;
namespace LibraryApp.Services
{
    public class BookService
    {
        public User user;
        public BookService(User user)
        {
            this.user = user;
        }
        public bool AddBook(Models.Book newBook)
        {
            bool status = false;
            string jsonString;
            List<Models.Book>? books;
            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");// need to download the DotNetEnv Nuget

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                books = new List<Models.Book>() { newBook };
                status = true;
            }
            else
            {
                books = JsonSerializer.Deserialize<List<Models.Book>>(File.ReadAllText(filePath)) ?? new List<Models.Book>();
                if (!books.Contains(newBook))
                {
                    books.Add(newBook);
                    status = true;
                    LogService.Log($"User: {user.GetId()} [ADDBOOK] New Book: {newBook.GetTitle()} created.", "books");
                }

            }

            jsonString = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath!, jsonString);
            return status;
        }

        public Models.Book? GetBook(Models.Book book)
        {
            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");

            List<Models.Book>? booksList = JsonSerializer.Deserialize<List<Models.Book>>(File.ReadAllText(filePath));
            if (booksList is not null && booksList.Contains(book))
            {
                foreach (var item in booksList)
                {
                    if (item == book)
                    {
                        book = item;
                        break;
                    }
                }

                LogService.Log($"User: {user.GetId()} [GETBOOK] Book: {book.GetTitle()} found.", "books");
            }
            else
            {
                LogService.Log($"User: {user.GetId()} [GETBOOK] Book: {book.GetTitle()} not found.", "books");
                book = null;
            }

            return book;
        }

        public static List<Models.Book>? GetAllBooks()
        {
            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");
            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Models.Book>();

            return JsonSerializer.Deserialize<List<Models.Book>>(File.ReadAllText(filePath)) ?? new List<Models.Book>();
        }

        public bool RemoveBook(Models.Book book)
        {
            bool status = false;
            List<Models.Book>? books = new List<Models.Book>();
            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                status = false;
            }
            else
            {
                books = JsonSerializer.Deserialize<List<Models.Book>>(File.ReadAllText(filePath)) ?? new List<Models.Book>();
                if (books.Contains(book))
                {
                    books.Remove(book);
                    status = true;
                    LogService.Log($"User: {user.GetId()} [REMOVEBOOK] Book: {book.GetTitle()} removed.", "books");
                }
            }

            string jsonString = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath!, jsonString);
            return status;
        }
        public bool UpdateBook(Models.Book originalBook, Models.Book updatedBook)
        {
            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");

            List<Models.Book>? booksList = JsonSerializer.Deserialize<List<Models.Book>>(File.ReadAllText(filePath));

            if (booksList is not null && booksList.Contains(originalBook))
            {
                int index = booksList.IndexOf(originalBook);
                booksList.RemoveAt(index);
                booksList.Insert(index, updatedBook);
                LogService.Log($"User: {user.GetId()} [UPDATEBOOK] Book: {originalBook} updated to {updatedBook}", "books");
            }
            else
            {
                LogService.Log($"User: {user.GetId()} [UPDATEBOOK] Book: {originalBook.GetTitle()} not found.", "books");
                return false;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(booksList, options);
            File.WriteAllText(filePath!, jsonString);
            return true;
        }

        public bool BookExists(string title, string author)
        {
            List<Book>? booksList = GetAllBooks();

            if (booksList is null || booksList.Count == 0)
                return false;

            bool exists = booksList.Contains(new Book(title, author));

            if (exists)
                LogService.Log($"User: {user.GetId()} [BOOKEXISTS] '{title}' by {author} found.", "books");
            else
                LogService.Log($"User: {user.GetId()} [BOOKEXISTS] '{title}' by {author} NOT found.", "books");

            return exists;
        }
    }
}
