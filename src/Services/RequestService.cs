using DotNetEnv;
using LibraryApp.Models;
using System.Text.Json;

namespace LibraryApp.Services;

public static class RequestService
{
    public static bool AddRequest(Models.Request newRequest)
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
                LogService.Log($"[ADDREQUEST] New Request: {newRequest} created.");
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
    public static List<Models.Request>? GetRequest(Models.User user)
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
                LogService.Log($"[GETREQUETS] All Requests of User: {user} found.");
            }
            else
            {
                LogService.Log($"[GETREQUETS] User: {user} does not have any requests");
                userRequests = null;
            }

        }
        else
        {
            LogService.Log($"[GETREQUETS] There are no Requests");
            userRequests = null;
        }
        return userRequests;
    }
    public static bool RemoveRequest(Models.Request request)
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
                LogService.Log($"[REMOVERequest] Request: {request} removed.");
            }
        }

        string jsonString = JsonSerializer.Serialize(requests, options);
        File.WriteAllText(filePath!, jsonString);
        return status;
    }
    public static bool UpdateRequest(Models.Request originalRequest, Models.Request updatedRequest)
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
            LogService.Log($"[UPDATEREQUEST] Request: {originalRequest} updated to {updatedRequest}");
        }
        else if (requestsList is not null && requestsList.Contains(updatedRequest))
        {
            LogService.Log($"[UPDATEREQUEST] Request: {updatedRequest} already exist found.");
            return false;
        }
        else
        {
            LogService.Log($"[UPDATEREQUEST] Request: {originalRequest} not found.");
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
