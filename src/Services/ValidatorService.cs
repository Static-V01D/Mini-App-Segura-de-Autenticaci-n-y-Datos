namespace LibraryApp.Services;

public static class ValidatorService
{
    public static bool ValidateUsername(string username)
    {
        // Username must be between 3 and 20 characters
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 20)
            return false;

        // Username can only contain letters, numbers, and underscores
        return username.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    public static bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
    }
}