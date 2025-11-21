using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DotNetEnv;
using LibraryApp.Models;
using LibraryApp.Services;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace LibraryApp

{
    class Program
    {
        static void Main(string[] args)
        {
            // Load environment variables from .env file
            Env.Load();

            Console.Title = "LibraryApp - Authentication System";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== LibraryApp Authentication System ===\n");
            Console.ResetColor();

            while (true)
            {
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("\nChoose an option: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterUser();
                        break;
                    case "2":
                        LoginUser();
                        break;
                    case "3":
                        Console.WriteLine("\nExiting program...");
                        return;
                    default:
                        Console.WriteLine("\nInvalid option. Try again.\n");
                        break;
                }
            }
        }

        private static void RegisterUser()
        {
            Console.Write("\nEnter username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Enter password: ");
            string password = Console.ReadLine() ?? "";

            Console.Write("Enter role (member/librarian): ");
            string role = Console.ReadLine() ?? "member";

            // Hash password before saving
            string hashedPassword = AuthService.HashPassword(password);

            var user = new Models.User(username, hashedPassword, role);

            bool success = AuthService.Register(user);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n User '{username}' registered successfully!\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  User '{username}' already exists.\n");
            }

            Console.ResetColor();
        }

        private static void LoginUser()
        {
            Console.Write("\nEnter username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Enter password: ");
            string password = Console.ReadLine() ?? "";

            // Hash password before checking
            string hashedPassword = AuthService.HashPassword(password);
            var user = new Models.User(username, hashedPassword);

            var loggedIn = AuthService.Login(user);

            if (loggedIn != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n Welcome, {loggedIn.GetName()}!");
                Console.WriteLine($"Role: {loggedIn.GetRole()}");

                if (AuthService.AuthorizedRoles(loggedIn.GetRole(), "librarian", "admin"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Access granted: Librarian/Admin functions enabled.\n");
                    LibrarianMenu(loggedIn);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Access limited: Member functions only.\n");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Invalid username or password.\n");
            }
            // --- MENUS ---

            static void LibrarianMenu(User user)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Books\n2. Add Book\n3. Update Book\n4. Delete Book\n5. Manage Loans\n0. Logout");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewBooks(); break;
                        case "2": AddBook(); break;
                        case "3": UpdateBook(); break;
                        case "4": DeleteBook(); break;
                        case "5": ManageLoans(user); break;
                        case "0": Log($"User {user} logged out."); return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            static void MemberMenu(User user)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Books\n2. Request Hold\n0. Logout");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewBooks(); break;
                        case "2": CheckoutBook(user); break;
                        case "0": Log($"User {user} logged out."); return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            // --- BOOK MANAGEMENT ---

            static void ViewBooks()
            {
                Console.WriteLine("\n=== Books ===");
                var books = BookService.GetAllBooks();
                if (books is not null)
                {
                    foreach (var book in books)
                    {
                        Console.WriteLine($"Title: {book.GetTitle()}, Author: {book.GetAuthor()}, Available: {book.IsAvailable()}");
                    }
                }
            }

            static void AddBook()
            {
                BookService.AddBook(new Book("One Piece", "Eiichiro Oda", true));
            }

            static void UpdateBook()
            {

            }

            static void DeleteBook()
            {

            }

            static void CheckoutBook(User user)
            {
                Console.Write("Enter Book Title to checkout: ");
                string title = Console.ReadLine() ?? "";
                Console.Write("Enter Book Author to checkout: ");
                string author = Console.ReadLine() ?? "";

                // BookService.CheckoutBook(new Book(title, author, true));
            }


            // --- LOAN MANAGEMENT ---

            static void ManageLoans(User user)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Loans\n2. Check Out\n3. Check In\n4. Update Loan\n0. Back");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewLoans(); break;
                        case "2": AddLoan(); break;
                        case "3": CheckIn(); break;
                        case "4": UpdateLoan(); break;                       
                        case "0": return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            static void ViewLoans()
            {

                string? filePath = Environment.GetEnvironmentVariable("LOANS_DB");
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new InvalidOperationException("LOANS_DB environment variable not found.");

                var json = File.ReadAllText(filePath);
                var loansList = JsonSerializer.Deserialize<List<Loan>>(json) ?? new();

                Console.WriteLine("\n=== Loans ===");
                foreach (var loan in loansList)
                {
                    Console.WriteLine($"LoanId: {loan.LoanId}, BookId: {loan.BookId}, MemberId: {loan.MemberId}, Status: {loan.Status}, DueDate: {loan.DueDate}, CheckoutDate: {loan.CheckoutDate}");
                }
            }

            static void AddLoan() //Hacer false available en book, Hacer que el LoanId sea auto incrementable
            {
                Env.Load();

                Console.Write("Book ID to check out: ");
                int bookId = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Member ID: ");
                int memberId = int.Parse(Console.ReadLine() ?? "0");

                // Create new loan
                var newLoan = new Loan
                {
                    BookId = bookId,
                    MemberId = memberId,
                    Status = "checked_out"
                };

                // Add the loan
                LoanService.AddLoan(newLoan);                

                // Update book availability
                string? booksPath = Environment.GetEnvironmentVariable("BOOKS_DB");
                if (string.IsNullOrWhiteSpace(booksPath))
                    throw new InvalidOperationException("BOOKS_DB environment variable not found.");

                var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(booksPath)) ?? new();
                var book = books.FirstOrDefault(b => b.GetId() == bookId);

                if (book != null)
                {
                    var updatedBook = new Book(book.GetTitle(), book.GetAuthor(), false); // false = not available
                    BookService.UpdateBook(book, updatedBook);
                    LogService.Log($"[CHECKOUT] Book ID {bookId} marked as not available.");
                }

                Console.WriteLine($"Book ID {bookId} checked out successfully!");
                Console.WriteLine($"Due Date: {newLoan.DueDate:dd-MM-yyyy}");
                Console.WriteLine($"Checkout Date: {newLoan.CheckoutDate:dd-MM-yyyy}");
                Console.WriteLine($"Loan ID: {newLoan.LoanId}" + $" Member ID: {newLoan.MemberId}" + $" Book ID: {newLoan.BookId}");                    
            }


        }

        static void CheckIn()
        {
            Env.Load();

            Console.Write("Book ID to check in: ");
            int bookId = int.Parse(Console.ReadLine() ?? "0");

            // Remove the loan referencing this Book
            bool removed = LoanService.RemoveLoan(new Loan { BookId = bookId });

            if (!removed)
            {
                Console.WriteLine($"Book ID {bookId} not found in loans.");
                return;
            }

            string? booksPath = Environment.GetEnvironmentVariable("BOOKS_DB");
            if (string.IsNullOrWhiteSpace(booksPath))
                throw new InvalidOperationException("BOOKS_DB env var not found.");

            var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(booksPath)) ?? new();
            var book = books.FirstOrDefault(b => b.GetId() == bookId);

            if (book != null)
            {
                var updatedBook = new Book(book.GetTitle(), book.GetAuthor(), true);
                BookService.UpdateBook(book, updatedBook);
                LogService.Log($"[CHECKIN] Book ID {bookId} marked as available.");
            }

            Console.WriteLine($"Book ID {bookId} checked in successfully!");
        }


        static void UpdateLoan() //Solo cambiar due date
        {
            Console.Write("Enter Loan ID: ");
            int loanId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter new due date (MM/DD/YYYY): ");
            DateOnly newDate = DateOnly.Parse(Console.ReadLine() ?? "");

            if (LoanService.UpdateLoan(loanId, newDate))
            {
                Console.WriteLine("Due date updated successfully!");
            }
            else
            {
                Console.WriteLine("Loan not found.");
            }
        }
        // --- LOGGING ---

        static void Log(string message)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";

        }
    }


}


