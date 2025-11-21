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
            string jsonString;
            List<Models.Loan>? loans;
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");

            Env.Load();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            var options = new JsonSerializerOptions { WriteIndented = true };

            // Set checkout date HERE
            newLoan.CheckoutDate = DateOnly.FromDateTime(DateTime.Now);

            // Ask user for due date HERE
            Console.Write("Enter due date (dd-MM-yyyy or dd/MM/yyyy): ");
            string input = Console.ReadLine()!;

            newLoan.DueDate = DateOnly.ParseExact(
                input,
                new[] { "dd-MM-yyyy", "dd/MM/yyyy" },
                null
            );

            // Load / create list
            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                newLoan.LoanId = 1;
                loans = new() { newLoan };
                status = true;
            }
            else
            {
                loans = JsonSerializer.Deserialize<List<Models.Loan>>(File.ReadAllText(filePath)) ?? new();

                newLoan.LoanId = loans.Any() ? loans.Max(l => l.LoanId) + 1 : 1;

                loans.Add(newLoan);
                status = true;
            }

            // Save JSON
            jsonString = JsonSerializer.Serialize(loans, options);
            File.WriteAllText(filePath!, jsonString);

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
