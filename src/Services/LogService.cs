using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using LibraryApp.Models;
using DotNetEnv;

namespace LibraryApp.Services
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }

    public class LogService
    {
        public User user;
        public LogService(User user)
        {
            this.user = user;
        }
        public static void Log(string message, string file)
        {
            Env.Load();
            string LogPath = "";
            switch (file)
            {
                case "users":
                    LogPath = Environment.GetEnvironmentVariable("USERSLOG_FILE");
                    break;
                case "requests":
                    LogPath = Environment.GetEnvironmentVariable("REQUESTSLOG_FILE");
                    break;
                case "books":
                    LogPath = Environment.GetEnvironmentVariable("BOOKSLOG_FILE");
                    break;
                case "loans":
                    LogPath = Environment.GetEnvironmentVariable("LOANSLOG_FILE");
                    break;
                default:
                    throw new ArgumentException(nameof(LogPath) + $"There is no log file for {file} ");

            }
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
