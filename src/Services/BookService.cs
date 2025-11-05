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
    }
}
