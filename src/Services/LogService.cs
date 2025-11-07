namespace LibraryApp.Services
{
    public static class LogService
    {
        private static readonly string LogPath = "bin/Debug/net.8.0/log.json";

        public static void Log(string message)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Directory.CreateDirectory("bin/Debug/net.8.0");
            File.AppendAllLines(LogPath, new[] { entry });
        }
    }
}
