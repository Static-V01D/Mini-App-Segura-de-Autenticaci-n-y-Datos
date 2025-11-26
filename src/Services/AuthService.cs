using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using LibraryApp.Models;


namespace LibraryApp.Services;

public static class AuthService
{
    // Convert password → SHA256 hash
    public static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash); // uppercase hex string
    }

    public static bool AuthorizedRoles(string userRole, params string[] allowedRoles)
    {
        for (int i = 0; i < allowedRoles.Length - 1; i++)
        {
            allowedRoles[i] = allowedRoles[i].ToLower();
        }
        return allowedRoles.Contains(userRole.ToLower());
    }

    // Register
    public static bool Register(Models.User user)
    {
        Env.Load();
        bool status = false;
        string jsonString;
        List<Models.User>? accounts;
        string? filePath = Environment.GetEnvironmentVariable("USERS_DB");// need to download the DotNetEnv Nuget

        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("USERS_DB environment variable not found.");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
        {
            accounts = new List<Models.User>() { user };
            status = true;
        }
        else
        {
            accounts = JsonSerializer.Deserialize<List<Models.User>>(File.ReadAllText(filePath)) ?? new List<Models.User>();
            if (!accounts.Contains(user))
            {
                accounts.Add(user);
                status = true;
            }

        }

        jsonString = JsonSerializer.Serialize(accounts, options);
        File.WriteAllText(filePath!, jsonString);
        LogService.Log($"[REGISTER] New user {user.GetId()} created.", "users");
        return status;
    }

    // Login
    public static Models.User? Login(Models.User user)
    {
        Env.Load();

        string? filePath = Environment.GetEnvironmentVariable("USERS_DB");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("USERS_DB environment variable not found.");
        if (!File.Exists(filePath))
        {
            using (File.Create(filePath)) { }
        }
        List<Models.User>? usersList = JsonSerializer.Deserialize<List<Models.User>>(File.ReadAllText(filePath));

        if (usersList is not null && usersList.Contains(user))
        {
            foreach (var account in usersList)
            {
                if (account == user)
                {
                    user = account;
                    break;
                }


            }

            LogService.Log($"[LOGIN] User {user.GetId()} logged in.", "users");

        }
        else
        {
            // Maybe another Log?
            LogService.Log($"[LOGIN] User {user.GetId()} tried to log in.", "users");
            user = null;
        }

        return user;
    }


}