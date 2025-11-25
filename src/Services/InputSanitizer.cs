using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LibraryApp.Utils
{
    public static class InputSanitizer
    {
        // Removes dangerous characters for JSON injection
        private static readonly Regex DangerousChars = new Regex(@"[{}[\]<>""\\]", RegexOptions.Compiled);

        public static string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Normalize unicode (prevents homograph attacks)
            string normalized = input.Normalize(NormalizationForm.FormC);

            // Remove characters that could break JSON structure
            normalized = DangerousChars.Replace(normalized, "");

            // Trim whitespace
            return normalized.Trim();
        }

        public static bool IsValidUsername(string input)
        {
            // Only letters, digits, underscores | 3–20 chars
            return Regex.IsMatch(input, @"^[A-Za-z0-9_]{3,20}$");
        }

        public static bool IsValidPassword(string input)
        {
            return input.Length >= 6 && input.Length <= 100;
        }
    }
}
