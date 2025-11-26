using System.Text.RegularExpressions;
using LibraryApp.Models;
using System.Text.Json;
using System.IO;
using DotNetEnv;
namespace LibraryApp.Services;


public static class ValidatorService
{
    // --- Regex patterns ---
    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9_.-]{1,50}$", RegexOptions.Compiled);
    private static readonly Regex PasswordPattern = new(@"^(?=.*[!@#^*\-+?_])[A-Za-z0-9!@#^*\-+?_]{12,30}$", RegexOptions.Compiled);///Can be changed to enforce more complexity
    private static readonly Regex RolePattern = new(@"^[a-zA-Z]{1,20}$", RegexOptions.Compiled);

    // --- Check if username is valid ---
    public static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return UsernamePattern.IsMatch(username);
    }

    // --- Check if password is valid ---
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return PasswordPattern.IsMatch(password);
    }

    // --- Check if role is valid ---
    public static bool IsValidRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return RolePattern.IsMatch(role);
    }

    private static readonly Regex BookFieldPattern =
    new(@"^[a-zA-Z0-9 .,'-]{1,100}$", RegexOptions.Compiled);

    public static bool IsValidBookField(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return BookFieldPattern.IsMatch(input);
    }


    // Optional: interactive input method (like before)
    public static string SafeStringInput()
    {
        while (true)
        {
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty.");
                continue;
            }

            return input;
        }
    }

    public static bool MemberIdExists(int memberId)
    {
        Env.Load();
        string? filePath = Environment.GetEnvironmentVariable("USERS_DB");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("USERS_DB environment variable not found.");
        List<Models.User>? users = JsonSerializer.Deserialize<List<Models.User>>(File.ReadAllText(filePath));

        return users.Any(u => u.GetId() == memberId);
    }
}

