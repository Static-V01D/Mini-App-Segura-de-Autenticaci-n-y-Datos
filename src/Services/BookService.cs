using LibraryApp.Models;


namespace LibraryApp.Services
{
    public static class BookService
    {
        private static readonly string BooksPath = "data/books.json";

        public static void ListBooks(List<Book> books)
        {
            Console.WriteLine("\n📖 Books Available:");
            foreach (var book in books)
                Console.WriteLine($"[{book.Id}] {book.Title} by {book.Author} {(book.Available ? "(Available)" : "(Checked out)")}");
        }

        public static void AddBook(List<Book> books)
        {
            Console.Write("Title: ");
            string title = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Author: ");
            string author = Console.ReadLine()?.Trim() ?? "";

            int id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(new Book { Id = id, Title = title, Author = author });
            DataStore.SaveList(BooksPath, books);

            LogService.Log($"[BOOK ADD] '{title}' added by librarian.");
        }

        public static void DeleteBook(List<Book> books)
        {
            Console.Write("Book ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            books.Remove(book);
            DataStore.SaveList(BooksPath, books);
            LogService.Log($"[BOOK DELETE] '{book.Title}' removed.");
        }
    }
}
