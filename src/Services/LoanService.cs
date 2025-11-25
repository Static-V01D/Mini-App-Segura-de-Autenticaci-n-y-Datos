using LibraryApp.Models;
using System.Text.Json.Serialization;
using DotNetEnv;
using LibraryApp.Services;
using System.Text.Json;
using Microsoft.VisualBasic;
namespace LibraryApp.Services
{
    public static class LoanService
    {        
        public static bool AddLoan(Models.Loan newLoan)
        {
            bool status = false;
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            Env.Load();
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Load / create list
            List<Models.Loan> loans;
            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                loans = new List<Models.Loan>();
            }
            else
            {
                loans = JsonSerializer.Deserialize<List<Models.Loan>>(File.ReadAllText(filePath)) ?? new List<Models.Loan>();
            }

            // Check if the book is already loaned
            if (loans.Any(l => l.BookId == newLoan.BookId && l.Status == "checked_out"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Book ID {newLoan.BookId} is already checked out!");
                Console.ResetColor();
                return false; // Stop adding
            }

            // Set LoanId
            newLoan.LoanId = loans.Any() ? loans.Max(l => l.LoanId) + 1 : 1;

            // Set checkout date
            newLoan.CheckoutDate = DateOnly.FromDateTime(DateTime.Now);

            // Ask user for due date
            Console.Write("Enter due date (dd-MM-yyyy or dd/MM/yyyy): ");
            string input = Console.ReadLine()!;
            newLoan.DueDate = DateOnly.ParseExact(input, new[] { "dd-MM-yyyy", "dd/MM/yyyy" }, null);

            // Add new loan
            loans.Add(newLoan);
            status = true;

            // Save JSON
            File.WriteAllText(filePath, JsonSerializer.Serialize(loans, options));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Book ID {newLoan.BookId} checked out successfully!");
            Console.ResetColor();

            return status;
        }

        public static bool UpdateLoan(int loanId, DateOnly newDueDate)
        {
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            var loans = JsonSerializer.Deserialize<List<Loan>>(File.ReadAllText(filePath)) ?? new();

            var loan = loans.FirstOrDefault(l => l.LoanId == loanId);
            if (loan == null)
                return false;

            // Only update due date
            loan.DueDate = newDueDate;

            File.WriteAllText(
                filePath,
                JsonSerializer.Serialize(loans, new JsonSerializerOptions { WriteIndented = true })
            );

            return true;
        }

        public static bool RemoveLoan(Loan loanToRemove)
        {
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB env var not found.");

            var loans = JsonSerializer.Deserialize<List<Loan>>(File.ReadAllText(filePath)) ?? new();

            // Match by BookId (or LoanId)
            var existing = loans.FirstOrDefault(l => l.BookId == loanToRemove.BookId);

            if (existing == null)
                return false;

            loans.Remove(existing);

            File.WriteAllText(filePath, JsonSerializer.Serialize(loans, new JsonSerializerOptions { WriteIndented = true }));

            LogService.Log($"[REMOVELOAN] Loan {existing.LoanId} removed.");
            return true;
        }


        public static Models.Loan? GetLoan(Models.Loan loan)
        {
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            var json = File.ReadAllText(filePath);
            var loansList = JsonSerializer.Deserialize<List<Models.Loan>>(json) ?? new();

            var foundLoan = loansList.FirstOrDefault(l =>
                l.LoanId == loan.LoanId &&
                l.BookId == loan.BookId &&
                l.MemberId == loan.MemberId
            );

            if (foundLoan != null)
            {
                LogService.Log($"[GETLOAN] Loan: {foundLoan.LoanId} found.");
                return foundLoan;
            }

            LogService.Log($"[GETLOAN] Loan: {loan.LoanId} not found.");
            return null;
        }

       
    }
}
