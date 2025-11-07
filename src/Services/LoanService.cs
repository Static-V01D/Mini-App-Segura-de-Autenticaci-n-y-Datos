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
        public static bool CheckOut(Models.Loan newLoan)
        {
            bool status = false;
            string jsonString;
            List<Models.Loan>? loans;
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");// need to download the DotNetEnv Nuget
            Env.Load();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                loans = new List<Models.Loan>() { newLoan };
                status = true;
            }
            else
            {
                loans = JsonSerializer.Deserialize<List<Models.Loan>>(File.ReadAllText(filePath)) ?? new List<Models.Loan>();
                if (!loans.Contains(newLoan))
                {
                    loans.Add(newLoan);
                    status = true;
                    LogService.Log($"[ADDLOAN] New Loan: {newLoan.LoanId} created.");
                }

            }

            jsonString = JsonSerializer.Serialize(loans, options);
            File.WriteAllText(filePath!, jsonString);
            return status;
        }

        public static bool UpdateLoan(Models.Loan updatedLoan)
        {
            bool status = false;
            string jsonString;
            List<Models.Loan>? loans;
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (File.Exists(filePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                loans = JsonSerializer.Deserialize<List<Models.Loan>>(File.ReadAllText(filePath)) ?? new List<Models.Loan>();
                for (int i = 0; i < loans.Count; i++)
                {
                    if (loans[i].LoanId == updatedLoan.LoanId)
                    {
                        loans[i] = updatedLoan;
                        status = true;
                        LogService.Log($"[UPDATELOAN] Loan: {updatedLoan.LoanId} updated.");
                        break;
                    }
                }

                jsonString = JsonSerializer.Serialize(loans, options);
                File.WriteAllText(filePath!, jsonString);
            }

            return status;
        }

        public static bool RemoveLoan(Models.Loan loanToRemove)
        {
            bool status = false;
            string jsonString;
            List<Models.Loan>? loans;
            string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("LOANS_DB environment variable not found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (File.Exists(filePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
            {
                loans = JsonSerializer.Deserialize<List<Models.Loan>>(File.ReadAllText(filePath)) ?? new List<Models.Loan>();
                if (loans.Contains(loanToRemove))
                {
                    loans.Remove(loanToRemove);
                    status = true;
                    LogService.Log($"[REMOVELOAN] Loan: {loanToRemove.LoanId} removed.");
                }

                jsonString = JsonSerializer.Serialize(loans, options);
                File.WriteAllText(filePath!, jsonString);
            }

            return status;
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
       public static void SetDueDate(Models.Loan dueDate, Models.Loan checkoutDate)
       {
           Console.Write("Enter Loan ID to set due date: ");
           int loanId = int.Parse(Console.ReadLine() ?? "0");
           Console.Write("Enter Due Date (yyyy-MM-dd): ");
           dueDate.DueDate = DateOnly.Parse(Console.ReadLine() ?? DateTime.Now.ToString()); // Hacer que solo sea Dia/mes/año
            Console.WriteLine("Due date is " + dueDate.DueDate);
            var today =  DateOnly.FromDateTime(DateTime.Now);
            checkoutDate.CheckoutDate = today;
           Console.WriteLine("Checkout date is " + today);
           
            if (UpdateLoan(dueDate))
            {
                Console.WriteLine("Due date updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update due date.");
            }
        }
    }
}
