using System.Security.Principal;
using System.Text.Json.Serialization;
namespace LibraryApp.Models;

public class User
{   
    [JsonInclude] private string name;
    [JsonInclude] private string password;// when passing the password to the constructer you must manually Hash the password with the static method Hash
    [JsonInclude] private string role;
    [JsonInclude] private List<LoanedBooks> loans;

    public User(string n, string pass, string r, List<LoanedBooks> l)
    {
        SetName(n);
        SetPassword(pass);
        SetRole(r);
        SetLoans(l);
    }
    public User(string n, string pass, string r) : this(n, pass, r, null)
    {
    }
    public User(string n, string pass, List<LoanedBooks>? l) : this(n, pass, null, l)
    {
    }
    public User(string n, string pass) : this(n, pass, null, null)
    {
    }
    public User() : this("guest", "123", null, null)
    {
    }

    public void SetName(string n)
    {
        if (n is null) throw new ArgumentNullException(nameof(n));

        //encript name? for anti SQLInyection
        else name = n;

    }
    public void SetRole(string r)
    {
        if (r is null) role = "member";

        else role = r;

    }
    public void SetPassword(string pass)
    {
        if (pass is null) throw new ArgumentNullException(nameof(pass));

        else password = pass;
    }
    public void SetLoans(List<LoanedBooks> l)
    {
        loans = l ?? new List<LoanedBooks>();
    }

    public string GetName() { return name; }// decript value for actual Name?
    public string GetPassword() { return password; }
    public string GetRole() { return role; }
    public List<LoanedBooks> GetLoans() { return loans; }

    public override string ToString()
    {
        return $"{GetType().Name}: User= {name}, role= {role}, password= {password},\n    {string.Join("\n    ", loans)}";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(name, password, role);
    }

    public bool Equals(User? other)
    {
        return this.Equals((Object?)other);
    }
    public override bool Equals(Object? obj)
    {
        return (obj is User other) && // Same type?
        (ReferenceEquals(this, other) || // Same references?
        (this.name == other.name && this.password == other.password && this.role == other.role)); // same properties
    }
    public static bool operator ==(User account1, User account2)
    {
        return account1 is null ? account2 is null : account1.Equals(account2);
    }
    public static bool operator !=(User account1, User account2)
    {
        return !(account1 == account2);
    }
}
