using System.Text.RegularExpressions;

namespace LibraryApp.Services;

public static class ValidatorService
{
    // --- Regex patterns ---
    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9_.-]{1,50}$", RegexOptions.Compiled);
    private static readonly Regex PasswordPattern = new(@"^[a-zA-Z0-9_.-]{12,30}$", RegexOptions.Compiled);
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

            if (!UsernamePattern.IsMatch(input))
            {
                Console.WriteLine("Invalid characters. Only letters, digits, spaces, _, -, and . allowed.");
                continue;
            }

            

            return input;
        }
    }    
}

