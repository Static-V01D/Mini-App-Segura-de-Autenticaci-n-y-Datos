using LibraryApp.Models;

namespace LibraryApp.Services
{
    public static class LoanService
    {
        private static readonly string LoansPath = "data/loans.json";
        private static readonly string BooksPath = "data/books.json";

        public static void CheckoutBook(List<Book> books, List<Loan> loans, LibraryApp.Models.User librarian)
        {
            Console.Write("Member ID: ");
            if (!int.TryParse(Console.ReadLine(), out int memberId)) return;

            Console.Write("Book ID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookId)) return;

            var book = books.FirstOrDefault(b => b.Id == bookId);
            if (book == null || !book.Available)
            {
                Console.WriteLine("Book not available.");
                return;
            }

            int loanId = loans.Any() ? loans.Max(l => l.LoanId) + 1 : 1;
            var loan = new Loan
            {
                LoanId = loanId,
                BookId = bookId,
                MemberId = memberId,
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "checked_out"
            };

            loans.Add(loan);
            book.Available = false;

            DataStore.SaveList(BooksPath, books);
            DataStore.SaveList(LoansPath, loans);
            LogService.Log($"[CHECKOUT] Book {bookId} loaned to Member {memberId} by {librarian.Username}");
        }

        public static void ReturnBook(List<Book> books, List<Loan> loans)
        {
            Console.Write("Loan ID to return: ");
            if (!int.TryParse(Console.ReadLine(), out int loanId)) return;

            var loan = loans.FirstOrDefault(l => l.LoanId == loanId);
            if (loan == null)
            {
                Console.WriteLine("Loan not found.");
                return;
            }

            loan.ReturnDate = DateTime.Now;
            loan.Status = loan.DueDate < DateTime.Now ? "overdue" : "returned";

            var book = books.First(b => b.Id == loan.BookId);
            book.Available = true;

            DataStore.SaveList(BooksPath, books);
            DataStore.SaveList(LoansPath, loans);
            LogService.Log($"[RETURN] Loan {loanId} completed. Status: {loan.Status}");
        }
    }
}
