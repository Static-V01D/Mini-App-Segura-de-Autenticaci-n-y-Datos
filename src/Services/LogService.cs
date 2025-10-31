namespace LibraryApp.Services
{
    public static class LogService
    {
        private static readonly string LogPath = "logs/audit.log";

        public static void Log(string message)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Directory.CreateDirectory("logs");
            File.AppendAllLines(LogPath, new[] { entry });
        }
    }
}
