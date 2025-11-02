using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DotNetEnv;


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

    // Register new user
    public static void Register(Models.User user)
    {
        Env.Load();
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

        }
        else
        {
            accounts = JsonSerializer.Deserialize<List<Models.User>>(File.ReadAllText(filePath)) ?? new List<Models.User>();
            accounts.Add(user);
        }

        jsonString = JsonSerializer.Serialize(accounts, options);
        File.WriteAllText(filePath!, jsonString);
        LogService.Log($"[REGISTER] New user {user.GetName()} created.");
    }

    // Login
    public static Models.User? Login(Models.User user)
    {

        Env.Load();
        string? filePath = Environment.GetEnvironmentVariable("USERS_DB");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("USERS_DB environment variable not found.");

        List<Models.User>? usersList = JsonSerializer.Deserialize<List<Models.User>>(File.ReadAllText(filePath));
        if (usersList.Contains(user))
        {
            LogService.Log($"[LOGIN] {user.GetName()} logged in.");
        }
        else
        {
            // Maybe another Log?
            LogService.Log($"[LOGIN] {user.GetName()} tried to log in.");
            user = null;
        }

        return user;
    }
}
