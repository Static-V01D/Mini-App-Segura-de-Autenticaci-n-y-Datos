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
            string username;
            do
            {
                Console.Write("\nEnter username: ");
                username = Console.ReadLine()?.Trim() ?? "";
                if (!ValidatorService.IsValidUsername(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid username! Use 1-50 letters, digits, _, -, or spaces.");
                    Console.ResetColor();
                }
            } while (!ValidatorService.IsValidUsername(username));

            string password;
            do
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine() ?? "";
                if (!ValidatorService.IsValidPassword(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid password! Must be 12+ chars, with uppercase, lowercase, number, or special character (! @ # ^ * _ - + ?).");
                    Console.ResetColor();
                }
            } while (!ValidatorService.IsValidPassword(password));


            // Hash password before saving
            string hashedPassword = AuthService.HashPassword(password);

            var user = new Models.User(username, hashedPassword, "member");

            bool success = AuthService.Register(user);

            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nUser '{username}' registered successfully!\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nUser '{username}' already exists.\n");
            }

            Console.ResetColor();
        }


        private static void LoginUser()
        {
            string username;
            do
            {
                Console.Write("\nEnter username: ");
                username = Console.ReadLine()?.Trim() ?? "";
                if (!ValidatorService.IsValidUsername(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid username! Use 1-50 letters, digits, _, -, or spaces.");
                    Console.ResetColor();
                }
            } while (!ValidatorService.IsValidUsername(username));

            string password;
            do
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine() ?? "";
                if (!ValidatorService.IsValidPassword(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid password! Must be 12+ chars, with uppercase, lowercase, number, or special character (! @ # ^ * _ - + ?).");
                    Console.ResetColor();
                }
            } while (!ValidatorService.IsValidPassword(password));

            // Hash password before checking
            string hashedPassword = AuthService.HashPassword(password);
            var user = new Models.User(username, hashedPassword);

            var loggedIn = AuthService.Login(user);

            if (loggedIn != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nWelcome, {loggedIn.GetName()}!");
                Console.WriteLine($"Role: {loggedIn.GetRole()}");

                BookService bookService = new BookService(user);
                LoanService loanService = new LoanService(user);
                RequestService requestService = new RequestService(user);
                LogService logService = new LogService(user);

                if (AuthService.AuthorizedRoles(loggedIn.GetRole(), "librarian", "admin"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Access granted: Librarian/Admin functions enabled.\n");
                    LibrarianMenu(loggedIn, bookService, requestService, loanService);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Access limited: Member functions only.\n");
                    MemberMenu(loggedIn, bookService, requestService);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Invalid username or password.\n");
                Console.ResetColor();
            }


            // --- MENUS ---

            void LibrarianMenu(User user, BookService bookService, RequestService requestService, LoanService loanService)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Books\n2. Add Book\n3. Update Book\n4. Delete Book\n5. Manage Loans\n6. Add Librarian\n7. Manage Requests\n0. Logout");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewBooks(); break;
                        case "2": AddBook(bookService); break;
                        case "3": UpdateBook(bookService); break;
                        case "4": DeleteBook(bookService); break;
                        case "5": ManageLoans(user, loanService, bookService); break;
                        case "6": AddLibrarian(); break;
                        case "7": ManageRequests(user, bookService, requestService); break;
                        case "0": Log($"User {user} logged out."); return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            void ManageRequests(User user, BookService bookService, RequestService requestService)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Requests\n2. Request Hold\n3. Remove Request\n4. Update Request\n0. Back");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": GetAllRequests(); break;
                        case "2": AddRequest(user, bookService, requestService); break;
                        case "3": RemoveRequest(user, requestService); break;
                        case "4": UpdateRequest(user, requestService); break;
                        case "0": return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            void GetAllRequests()
            {
                var requests = RequestService.GetAllRequests();
                if (requests is not null)
                {
                    foreach (var request in requests)
                    {
                        Console.WriteLine($"User: {request.GetUser().GetName()} ID: {request.GetUser().GetId()}, Book: {request.GetBook().GetTitle()} by {request.GetBook().GetAuthor()}");
                    }
                }
            }

            void GetRequest(Models.User user, RequestService requestService)
            {
                var requests = requestService.GetRequest(user);
                if (requests is not null)
                {
                    foreach (var request in requests)
                    {
                        Console.WriteLine($"User: {request.GetUser().GetName()}, Book: {request.GetBook().GetTitle()} by {request.GetBook().GetAuthor()}");
                    }
                }
            }

            void UpdateRequest(Models.User user, RequestService requestService)
            {
                Console.Write("Enter existing Book Title: ");
                string oldTitle = Console.ReadLine() ?? "";
                Console.Write("Enter existing Book Author: ");
                string oldAuthor = Console.ReadLine() ?? "";

                Console.Write("Enter NEW Book Title: ");
                string newTitle = Console.ReadLine() ?? "";
                Console.Write("Enter NEW Book Author: ");
                string newAuthor = Console.ReadLine() ?? "";

                var oldRequest = new Models.Request(user, new Book(oldTitle, oldAuthor));
                var newRequest = new Models.Request(user, new Book(newTitle, newAuthor));

                bool success = requestService.UpdateRequest(oldRequest, newRequest);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nRequest updated successfully!\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nFailed to update request.\n");
                }

                Console.ResetColor();
            }

            void RemoveRequest(Models.User user, RequestService requestService)
            {
                Console.Write("Enter Book Title to remove request: ");
                string title = Console.ReadLine() ?? "";
                Console.Write("Enter Book Author to remove request: ");
                string author = Console.ReadLine() ?? "";

                var request = new Models.Request(user, new Book(title, author));
                bool success = requestService.RemoveRequest(request);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nRequest for '{title}' by {author} removed successfully!\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nFailed to remove request for '{title}' by {author}.\n");
                }

                Console.ResetColor();
            }

            void AddLibrarian()
            {
                string username;
                do
                {
                    Console.Write("\nEnter librarian username: ");
                    username = Console.ReadLine()?.Trim() ?? "";
                    if (!ValidatorService.IsValidUsername(username))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid username! Use 1-50 letters, digits, _, -, or spaces.");
                        Console.ResetColor();
                    }
                } while (!ValidatorService.IsValidUsername(username));

                string password;
                do
                {
                    Console.Write("Enter librarian password: ");
                    password = Console.ReadLine() ?? "";
                    if (!ValidatorService.IsValidPassword(password))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid password! Must be 12+ chars, with uppercase, lowercase, number, or special character (! @ # ^ * _ - + ?).");
                        Console.ResetColor();
                    }
                } while (!ValidatorService.IsValidPassword(password));

                // Hash password before saving
                string hashedPassword = AuthService.HashPassword(password);

                var user = new Models.User(username, hashedPassword, "librarian");

                bool success = AuthService.Register(user);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nLibrarian '{username}' registered successfully!\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nLibrarian '{username}' already exists.\n");
                }

                Console.ResetColor();
            }

            // --- MEMBER MENU ---
            void MemberMenu(User user, BookService bookService, RequestService requestService)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Books\n2. View Requests\n3. Request Book\n0. Logout");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewBooks(); break;
                        case "2": GetRequest(user, requestService); break;
                        case "3": AddRequest(user, bookService, requestService); break;
                        case "0": Log($"User: Id={user.GetId()} Name={user.GetName()} logged out."); return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
            }

            // --- BOOK MANAGEMENT ---

            void ViewBooks()
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

            void AddBook(BookService bookService)
            {
                Console.WriteLine("Enter book title:");
                string title = Console.ReadLine() ?? "";
                Console.WriteLine("Enter book author:");
                string author = Console.ReadLine() ?? "";
                Console.WriteLine("Is the book available? (yes/no):");
                string availabilityInput = Console.ReadLine() ?? "yes";
                bool isAvailable = availabilityInput.ToLower() == "yes";

                bookService.AddBook(new Book(title, author, isAvailable));
            }

            void UpdateBook(BookService bookService)
            {
                Console.WriteLine("Enter current book title to update:");
                string currentTitle = Console.ReadLine() ?? "";
                Console.WriteLine("Enter current book author to update:");
                string currentAuthor = Console.ReadLine() ?? "";
                Book existingBook = bookService.GetBook(new Book(currentTitle, currentAuthor)) ?? new Book();

                Console.WriteLine("Enter new book title:");
                string newTitle = Console.ReadLine() ?? "";
                Console.WriteLine("Enter new book author:");
                string newAuthor = Console.ReadLine() ?? "";
                Console.WriteLine("Is the book available? (yes/no):");
                string availabilityInput = Console.ReadLine() ?? "yes";
                bool isAvailable = availabilityInput.ToLower() == "yes";

                Book updatedBook = new Book(newTitle, newAuthor, isAvailable);
                bookService.UpdateBook(existingBook, updatedBook);
            }

            static void DeleteBook(BookService bookService)
            {
                Console.WriteLine("Enter book title to delete:");
                string title = Console.ReadLine() ?? "";
                Console.WriteLine("Enter book author to delete:");
                string author = Console.ReadLine() ?? "";
                bookService.RemoveBook(new Book(title, author));
            }

            static void CheckoutBook(User user, BookService bookService)
            {
                Console.Write("Enter Book Title to checkout: ");
                string title = Console.ReadLine() ?? "";
                bookService.GetBook(new Book(title, ""));
                Console.Write("Enter Book Author to checkout: ");
                string author = Console.ReadLine() ?? "";
                bookService.GetBook(new Book("", author));

            }
            // --- REQUEST HOLD ---
            static void AddRequest(User user, BookService bookService, RequestService requestService)
            {
                Console.Write("Enter Book Title to request hold: ");
                string title = ValidatorService.SafeStringInput();

                Console.Write("Enter Book Author to request hold: ");
                string author = ValidatorService.SafeStringInput();

                // Validate title & author formatting
                if (!ValidatorService.IsValidBookField(title) || !ValidatorService.IsValidBookField(author))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid title or author format.\n");
                    Console.ResetColor();
                    return;
                }

                // Check database for book existence
                if (!bookService.BookExists(title, author))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nBook '{title}' by {author} does NOT exist in the database.\n");
                    Console.ResetColor();
                    return;
                }

                // Create the request
                var request = new Models.Request(user, new Book(title, author));

                bool success = requestService.AddRequest(request);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nHold request for '{title}' by {author} submitted successfully!\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nFailed to submit hold request for '{title}' by {author}.\n");
                }

                Console.ResetColor();
            }



            // --- LOAN MANAGEMENT ---

            static void ManageLoans(User user, LoanService loanService, BookService bookService)
            {
                while (true)
                {
                    Console.WriteLine("\n1. View Loans\n2. Check Out\n3. Check In\n4. Update Loan\n0. Back");
                    Console.Write("Choose: ");
                    string choice = Console.ReadLine() ?? "";
                    switch (choice)
                    {
                        case "1": ViewLoans(); break;
                        case "2": AddLoan(loanService); break;
                        case "3": CheckIn(loanService, bookService); break;
                        case "4": UpdateLoan(loanService); break;
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

            static void AddLoan(LoanService loanService)
            {
                Env.Load();

                // Load books from JSON
                string? booksPath = Environment.GetEnvironmentVariable("BOOKS_DB");
                if (string.IsNullOrWhiteSpace(booksPath))
                    throw new InvalidOperationException("BOOKS_DB environment variable not found.");

                var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(booksPath)) ?? new();

                Book? selectedBook = null;

                // Ask for book until a valid, available one is entered
                while (selectedBook == null)
                {
                    Console.Write("Enter Book Title: ");
                    string title = Console.ReadLine() ?? "";

                    Console.Write("Enter Book Author: ");
                    string author = Console.ReadLine() ?? "";

                    selectedBook = books.FirstOrDefault(b =>
                        b.GetTitle().Equals(title, StringComparison.OrdinalIgnoreCase) &&
                        b.GetAuthor().Equals(author, StringComparison.OrdinalIgnoreCase));

                    if (selectedBook == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Book not found. Please enter a valid title and author.");
                        Console.ResetColor();
                    }
                    else if (!selectedBook.IsAvailable())
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("This book is already checked out.");
                        Console.ResetColor();
                        selectedBook = null;
                    }
                }


                int memberId;
                while (true)
                {
                    Console.Write("Member ID: ");
                    if (!int.TryParse(Console.ReadLine(), out memberId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input. Please enter a number.");
                        Console.ResetColor();
                        continue;
                    }
                    if (!ValidatorService.MemberIdExists(memberId))
                    {
                        Console.WriteLine($"Member ID {memberId} does not exist.");
                        Console.ResetColor();
                        continue;
                    }
                    break;
                }

                // Create new loan
                var newLoan = new Loan
                {
                    BookId = selectedBook.GetId(),
                    MemberId = memberId,
                    Status = "checked_out"
                };

                // Add the loan
                if (loanService.AddLoan(newLoan))
                {
                    // Update book availability
                    var book = books.First(b => b.GetId() == selectedBook.GetId());
                    var updatedBook = new Book(book.GetTitle(), book.GetAuthor(), false); // false = not available
                    LogService.Log($"[CHECKOUT] Book '{selectedBook.GetTitle()}' by {selectedBook.GetAuthor()} checked out.", "loans");
                    Console.WriteLine($"Book '{selectedBook.GetTitle()}' checked out successfully!");
                }
            }



        }

        static void CheckIn(LoanService loanService, BookService bookService)
        {
            Env.Load();

            Console.Write("Book ID to check in: ");
            int bookId = int.Parse(Console.ReadLine() ?? "0");

            // Remove the loan referencing this Book
            bool removed = loanService.RemoveLoan(new Loan { BookId = bookId });

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
                bookService.UpdateBook(book, updatedBook);
                LogService.Log($"[CHECKIN] Book ID {bookId} marked as available.", "loans");
            }

            Console.WriteLine($"Book ID {bookId} checked in successfully!");
        }


        static void UpdateLoan(LoanService loanService)
        {
            Console.Write("Enter Loan ID: ");
            string? loanIdInput = Console.ReadLine();

            if (!int.TryParse(loanIdInput, out int loanId) || loanId <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Loan ID.");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter new due date (dd-MM-yyyy or dd/MM/yyyy): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Date cannot be empty.");
                Console.ResetColor();
                return;
            }

            // Try parsing with the same formats used in AddLoan
            DateOnly newDate;
            try
            {
                newDate = DateOnly.ParseExact(
                    input,
                    new[] { "dd-MM-yyyy", "dd/MM/yyyy" },
                    null
                );
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid date format. Use dd-MM-yyyy or dd/MM/yyyy.");
                Console.ResetColor();
                return;
            }

            if (loanService.UpdateLoan(loanId, newDate))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Due date updated successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Loan not found.");
                Console.ResetColor();
            }
        }


        // --- LOGGING ---

        static void Log(string message)
        {
            LogService.Log(message, "users");


        }
    }


}


