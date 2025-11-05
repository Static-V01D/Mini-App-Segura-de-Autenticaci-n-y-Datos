using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DotNetEnv;
using LibraryApp.Models;
using LibraryApp.Services;

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
                Console.WriteLine($"\n✅ User '{username}' registered successfully!\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n⚠️  User '{username}' already exists.\n");
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
                Console.WriteLine($"\n✅ Welcome, {loggedIn.GetName()}!");
                Console.WriteLine($"Role: {loggedIn.GetRole()}");

                if (AuthService.AuthorizedRoles(loggedIn.GetRole(), "librarian", "admin"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Access granted: Librarian/Admin functions enabled.\n");
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
                Console.WriteLine("\n❌ Invalid username or password.\n");
            }
            // --- MENUS ---

           /* static void LibrarianMenu(User user)
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
                        case "0": Log($"User {user.Username} logged out."); return;
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
                        case "2": RequestHold(user); break;
                        case "0": Log($"User {user.Username} logged out."); return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }*/

            // --- BOOK MANAGEMENT ---

            static void ViewBooks()
            {
                Book.GetBook(new Book("Lord of the Rings","J.R.R. Tolkien", true));
            }/*

            static void AddBook()
            {
                Console.Write("Book ID: ");
                string id = Console.ReadLine() ?? "";
                Console.Write("Title: ");
                string title = Console.ReadLine() ?? "";
                Console.Write("Author: ");
                string author = Console.ReadLine() ?? "";

                File.AppendAllText(booksFile, $"{id},{title},{author}\n");
                Log($"Added book '{title}' by {author}");
            }

            static void UpdateBook()
            {
                var books = File.ReadAllLines(booksFile).ToList();
                Console.Write("Enter book ID to update: ");
                string id = Console.ReadLine() ?? "";
                int index = books.FindIndex(b => b.StartsWith(id + ","));
                if (index == -1)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                Console.Write("New title: ");
                string title = Console.ReadLine() ?? "";
                Console.Write("New author: ");
                string author = Console.ReadLine() ?? "";
                books[index] = $"{id},{title},{author}";
                File.WriteAllLines(booksFile, books);
                Log($"Updated book {id}");
            }

            static void DeleteBook()
            {
                var books = File.ReadAllLines(booksFile).ToList();
                Console.Write("Enter book ID to delete: ");
                string id = Console.ReadLine() ?? "";
                int countBefore = books.Count;
                books = books.Where(b => !b.StartsWith(id + ",")).ToList();
                File.WriteAllLines(booksFile, books);
                if (books.Count < countBefore) Log($"Deleted book {id}");
                else Console.WriteLine("Book not found.");
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
                        case "2": CheckOut(); break;
                        case "3": CheckIn(); break;
                        case "0": return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            static void ViewLoans()
            {
                var loans = File.ReadAllLines(loansFile);
                Console.WriteLine("\n=== Loans ===");
                foreach (var l in loans)
                    Console.WriteLine(l);
            }

            static void CheckOut()
            {
                Console.Write("Member username: ");
                string member = Console.ReadLine() ?? "";
                Console.Write("Book ID: ");
                string bookId = Console.ReadLine() ?? "";

                File.AppendAllText(loansFile, $"{member},{bookId},{DateTime.Now:yyyy-MM-dd},NotReturned\n");
                Log($"Book {bookId} checked out by {member}");
            }

            static void CheckIn()
            {
                var loans = File.ReadAllLines(loansFile).ToList();
                Console.Write("Book ID to check in: ");
                string bookId = Console.ReadLine() ?? "";

                for (int i = 0; i < loans.Count; i++)
                {
                    if (loans[i].Contains($"{bookId},") && loans[i].EndsWith("NotReturned"))
                    {
                        loans[i] = loans[i].Replace("NotReturned", "Returned");
                        File.WriteAllLines(loansFile, loans);
                        Log($"Book {bookId} checked in");
                        return;
                    }
                }
                Console.WriteLine("No matching loan found.");
            }

            static void RequestHold(User user)
            {
                Console.Write("Book ID to request: ");
                string bookId = Console.ReadLine() ?? "";
                Log($"User {user.Username} requested hold on book {bookId}");
                Console.WriteLine("Hold request logged.");
            }

            // --- LOGGING ---

            static void Log(string message)
            {
                string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
                File.AppendAllText(logFile, entry + "\n");
            }
        }

        class User
        {
            public int Id { get; set; }
            public string Username { get; set; } = "";
            public string Role { get; set; } = "member";
            public string PasswordHash { get; set; } = "";
        }*/
        }
    }
}
