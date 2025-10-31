using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryApp.Services
{
    public static class DataStore
    {
        private static readonly JsonSerializerOptions options = new() { WriteIndented = true };

          // Load list of objects from a JSON file
        public static List<T> LoadList<T>(string path)
        {
            if (!File.Exists(path)) return new List<T>();

            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
            }
            catch (Exception ex)
            {
                LogService.Log($"[ERROR] Failed to read {path}: {ex.Message}");
                return new List<T>();
            }
        }

        // Save list of objects to a JSON file
        public static void SaveList<T>(string path, List<T> list)
        {
            try
            {
                string json = JsonSerializer.Serialize(list, options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                LogService.Log($"[ERROR] Failed to save {path}: {ex.Message}");
            }
        }
    }
}