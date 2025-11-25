using LibraryApp.Models;
using System.Text.Json.Serialization;
using DotNetEnv;
using LibraryApp.Services;
using System.Text.Json;
namespace LibraryApp.Services
{
    public static class BookService
    {
        public static bool AddBook(Models.Book newBook)
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
                    LogService.Log($"[ADDBOOK] New Book: {newBook.GetTitle()} created.");
                }

            }

            jsonString = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath!, jsonString);
            return status;
        }

        public static Models.Book? GetBook(Models.Book book)
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

                LogService.Log($"[GETBOOK] Book: {book.GetTitle()} found.");
            }
            else
            {
                LogService.Log($"[GETBOOK] Book: {book.GetTitle()} not found.");
                book = null;
            }

            return book;
        }

        public static List<Models.Book>? GetAllBooks()
        {
            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");

            return JsonSerializer.Deserialize<List<Models.Book>>(File.ReadAllText(filePath));
        }

        public static bool RemoveBook(Models.Book book)
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
                    LogService.Log($"[REMOVEBOOK] Book: {book.GetTitle()} removed.");
                }
            }

            string jsonString = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath!, jsonString);
            return status;
        }
        public static bool UpdateBook(Models.Book originalBook, Models.Book updatedBook)
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
                LogService.Log($"[UPDATEBOOK] Book: {originalBook} updated to {updatedBook}");
            }
            else
            {
                LogService.Log($"[UPDATEBOOK] Book: {originalBook.GetTitle()} not found.");
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

        public static bool BookExists(string title, string author)
        {
            Env.Load();

            string? filePath = Environment.GetEnvironmentVariable("BOOKS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("BOOKS_DB environment variable not found.");

            if (!File.Exists(filePath))
                return false;

            List<Book>? booksList = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(filePath));

            if (booksList is null || booksList.Count == 0)
                return false;

            bool exists = booksList.Any(b =>
                b.GetTitle().Equals(title, StringComparison.OrdinalIgnoreCase) &&
                b.GetAuthor().Equals(author, StringComparison.OrdinalIgnoreCase)
            );

            if (exists)
                LogService.Log($"[BOOKEXISTS] '{title}' by {author} found.");
            else
                LogService.Log($"[BOOKEXISTS] '{title}' by {author} NOT found.");

            return exists;
        }
    }
}
