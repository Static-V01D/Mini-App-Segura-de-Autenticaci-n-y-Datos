using System.Security.Cryptography;
using System.Text;
using LibraryApp.Models;

namespace LibraryApp.Services;
    public class AuthService
    {
        private static readonly string UsersPath = "data/users.json";

        // Convert password → SHA256 hash
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); // uppercase hex string
        }

        // Register new user
        public void Register(List<LibraryApp.Models.User> users)
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.Trim() ?? "";
            if (!ValidatorService.ValidateUsername(username))
            {
                Console.WriteLine("Invalid username format.");
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine()?.Trim() ?? "";
            if (!ValidatorService.ValidatePassword(password))
            {
                Console.WriteLine("Password too weak.");
                return;
            }

            string hash = HashPassword(password);
            int newId = users.Any() ? users.Max(u => u.Id) + 1 : 1;

            users.Add(new LibraryApp.Models.User { Id = newId, Username = username, PasswordHash = hash, Role = "member" });
            DataStore.SaveList(UsersPath, users);
            LogService.Log($"[REGISTER] New user {username} created.");
        }

        // Login
        public LibraryApp.Models.User? Login(List<LibraryApp.Models.User> users)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Password: ");
            string password = Console.ReadLine()?.Trim() ?? "";

            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                LogService.Log($"[LOGIN FAIL] Invalid credentials for {username}");
                Console.WriteLine("Invalid username or password.");
                return null;
            }

            LogService.Log($"[LOGIN] {username} logged in.");
            return user;
        }
    }
