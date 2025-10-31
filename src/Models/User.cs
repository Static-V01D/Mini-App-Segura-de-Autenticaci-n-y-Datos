namespace LibraryApp.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "member"; // "member" or "librarian"

    public User() { }

    public User(int id, string username, string passwordHash, string role = "member")
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}
