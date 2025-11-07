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
                    Console.WriteLine("\n1. View Loans\n2. Check Out\n3. Check In\n0. Back");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewLoans(); break;
                      //  case "2": AddLoan(); break;
                        case "3": CheckIn(); break;
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
            
           /* static void AddLoan() //Hacer false available en book, Hacer que el LoanId sea auto incrementable
            {
                Console.Write("Book ID to check out: ");
                int bookId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Enter Due Date (yyyy-MM-dd): ");
                Loan.DueDate = DateOnly.Parse(Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd"));
                Console.WriteLine("Due date is " + Loan.DueDate);

                var today = DateOnly.FromDateTime(DateTime.Now);
                Loan.CheckoutDate = today;
                Console.WriteLine("Checkout date is " + today);

                if (UpdateLoan(Loan))
                    Console.WriteLine("Due date updated successfully.");
                else
                    Console.WriteLine("Failed to update due date.");
            }*/

            static void CheckIn()//Hacer true available en book
            {

                Console.Write("Book ID to check in: ");
                int bookId = int.Parse(Console.ReadLine() ?? "0");
                LoanService.RemoveLoan(new Loan { LoanId = 1, BookId = bookId, MemberId = 1 });

            }

            static void UpdateLoan() //Solo cambiar due date
            {
                Console.Write("Enter Loan ID to update: ");
                int loanId = int.Parse(Console.ReadLine() ?? "0");
                LoanService.UpdateLoan(new Loan { LoanId = loanId, BookId = 1, MemberId = 1 });
            }
            // --- LOGGING ---

            static void Log(string message)
            {
                string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";

            }
        }


    }
}

