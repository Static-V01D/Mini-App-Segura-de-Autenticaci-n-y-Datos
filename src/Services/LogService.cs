using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace LibraryApp.Services
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }

    public static class LogService
    {
        private static readonly string LogPath = "bin/Debug/net8.0/log.json";

        public static void Log(string message)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);

            List<LogEntry> logs;

            // Read existing logs
            if (File.Exists(LogPath))
            {
                try
                {
                    string json = File.ReadAllText(LogPath);
                    logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
                }
                catch
                {
                    logs = new List<LogEntry>(); // Recover if corrupted
                }
            }
            else
            {
                logs = new List<LogEntry>();
            }

            // Add new entry
            logs.Add(new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message
            });

            // Write back as JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(LogPath, JsonSerializer.Serialize(logs, options));
        }
    }
}
