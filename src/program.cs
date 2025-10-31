using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace LibraryApp

{
    class Program
    {
        static string usersFile = "users.txt";
        static string booksFile = "books.txt";
        static string loansFile = "loans.txt";
        static string logFile = "log.txt";

        static void Main(string[] args)
        {
            Console.Title = "Library Book Borrowing System";
            Console.WriteLine("=== Library System ===");

            EnsureFilesExist();

            User? currentUser = null;

            while (currentUser == null)
            {
                Console.WriteLine("\n1. Register\n2. Login\n0. Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        Register();
                        break;
                    case "2":
                        currentUser = Login();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }

            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // If we can't clear the console, just add some blank lines instead
                Console.WriteLine("\n\n");
            }
            Console.WriteLine($"Welcome, {currentUser.Username}! Role: {currentUser.Role}");

            if (currentUser.Role == "librarian")
                LibrarianMenu(currentUser);
            else
                MemberMenu(currentUser);
        }

        static void EnsureFilesExist()
        {
            foreach (var file in new[] { usersFile, booksFile, loansFile, logFile })
                if (!File.Exists(file)) File.WriteAllText(file, "");
        }

        // --- AUTHENTICATION ---

        static void Register()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine() ?? "";
            Console.Write("Enter password: ");
            string password = Console.ReadLine() ?? "";
            Console.Write("Enter role (member/librarian): ");
            string role = Console.ReadLine() ?? "member";

            if (File.ReadAllLines(usersFile).Any(l => l.Split(',')[0] == username))
            {
                Console.WriteLine("User already exists.");
                return;
            }

            string hash = HashPassword(password);
            File.AppendAllText(usersFile, $"{username},{hash},{role}\n");
            Console.WriteLine("User registered successfully.");
        }

        static User? Login()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            string hash = HashPassword(password);
            foreach (var line in File.ReadAllLines(usersFile))
            {
                var parts = line.Split(',');
                if (parts.Length == 3 && parts[0] == username && parts[1] == hash)
                {
                    Log($"User {username} logged in.");
                    return new User { Username = username, Role = parts[2] };
                }
            }

            Console.WriteLine("Invalid username or password.");
            return null;
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
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
        }

        // --- BOOK MANAGEMENT ---

        static void ViewBooks()
        {
            var books = File.ReadAllLines(booksFile);
            Console.WriteLine("\n=== Books ===");
            foreach (var b in books)
                Console.WriteLine(b);
        }

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
    }
}
