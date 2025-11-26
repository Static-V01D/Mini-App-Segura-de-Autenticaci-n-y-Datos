using DotNetEnv;
using LibraryApp.Models;
using System.Text.Json;

namespace LibraryApp.Services;

public class RequestService
{

    public User user;
    public RequestService(User user)
    {
        this.user = user;
    }
    public bool AddRequest(Models.Request newRequest)
    {
        Env.Load();

        bool status = false;
        string jsonString;
        List<Models.Request>? requests;
        string? filePath = Environment.GetEnvironmentVariable("REQUEST_DB");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("REQUEST_DB environment variable not found.");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
        {
            requests = new List<Models.Request>() { newRequest };
            status = true;
        }
        else
        {
            requests = JsonSerializer.Deserialize<List<Models.Request>>(File.ReadAllText(filePath)) ?? new List<Models.Request>();
            if (!requests.Contains(newRequest))
            {
                requests.Add(newRequest);
                status = true;
                LogService.Log($"User: {user.GetId()} [ADDREQUEST] New Request: {newRequest} created.", "requests");
            }

        }

        jsonString = JsonSerializer.Serialize(requests, options);
        File.WriteAllText(filePath!, jsonString);
        return status;
    }
    public static List<Models.Request>? GetAllRequests()
    {
        Env.Load();
        string? filePath = Environment.GetEnvironmentVariable("REQUEST_DB");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("REQUEST_DB environment variable not found.");

        return JsonSerializer.Deserialize<List<Models.Request>>(File.ReadAllText(filePath));
    }
    public List<Models.Request>? GetRequest(Models.User user)
    {
        Env.Load();

        List<Models.Request>? requestsList = GetAllRequests();
        List<Models.Request>? userRequests = new List<Request>();
        if (requestsList is not null)
        {
            foreach (var req in requestsList)
            {
                if (req.GetUser() == user)
                {
                    userRequests.Add(req);
                }
            }
            if (userRequests is not null)
            {
                LogService.Log($"User: {user.GetId()} [GETREQUETS] All Requests of User: {user} found.", "requests");
            }
            else
            {
                LogService.Log($"User: {user.GetId()} [GETREQUETS] User: {user} does not have any requests", "requests");
                userRequests = null;
            }

        }
        else
        {
            LogService.Log($"User: {user.GetId()} [GETREQUETS] There are no Requests", "requests");
            userRequests = null;
        }
        return userRequests;
    }
    public bool RemoveRequest(Models.Request request)
    {
        Env.Load();

        bool status = false;
        List<Models.Request>? requests = new List<Models.Request>();
        string? filePath = Environment.GetEnvironmentVariable("REQUEST_DB");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("REQUEST_DB environment variable not found.");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
        {
            status = false;
        }
        else
        {
            requests = JsonSerializer.Deserialize<List<Models.Request>>(File.ReadAllText(filePath)) ?? new List<Models.Request>();
            if (requests.Contains(request))
            {
                requests.Remove(request);
                status = true;
                LogService.Log($"User: {user.GetId()} [REMOVERequest] Request: {request} removed.", "requests");
            }
        }

        string jsonString = JsonSerializer.Serialize(requests, options);
        File.WriteAllText(filePath!, jsonString);
        return status;
    }
    public bool UpdateRequest(Models.Request originalRequest, Models.Request updatedRequest)
    {
        Env.Load();

        string? filePath = Environment.GetEnvironmentVariable("REQUEST_DB");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("REQUEST_DB environment variable not found.");

        List<Models.Request>? requestsList = JsonSerializer.Deserialize<List<Models.Request>>(File.ReadAllText(filePath));

        if (requestsList is not null && requestsList.Contains(originalRequest) && !requestsList.Contains(updatedRequest))
        {
            int index = requestsList.IndexOf(originalRequest);
            requestsList.RemoveAt(index);
            requestsList.Insert(index, updatedRequest);
            LogService.Log($"User: {user.GetId()} [UPDATEREQUEST] Request: {originalRequest} updated to {updatedRequest}", "requests");
        }
        else if (requestsList is not null && requestsList.Contains(updatedRequest))
        {
            LogService.Log($"User: {user.GetId()} [UPDATEREQUEST] Request: {updatedRequest} already exist found.", "requests");
            return false;
        }
        else
        {
            LogService.Log($"User: {user.GetId()} [UPDATEREQUEST] Request: {originalRequest} not found.", "requests");
            return false;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        string jsonString = JsonSerializer.Serialize(requestsList, options);
        File.WriteAllText(filePath!, jsonString);
        return true;
    }

}
