using Microsoft.EntityFrameworkCore;

public static class Authorization
{
    public static async Task<bool> AuthorizeUser(string api_key, string method, DatabaseContext context)
    {
        var apiKeys = await context.Api_Keys.ToListAsync();
        var instance_apikey = apiKeys.FirstOrDefault(instance => RequestPermission(instance, api_key, method));

        if (instance_apikey == null) { return false; }
        return true;
    }

    public static bool RequestPermission(Api_Key instance, string api_key, string method)
    {
        if (instance.App == "dashboard" && BCrypt.Net.BCrypt.Verify(api_key, instance.ApiKey)) { return true; }
        return instance.App == "scanner" && BCrypt.Net.BCrypt.Verify(api_key, instance.ApiKey) && method == "GET";
    }

    public static int ValidateWarehouse(string api_key, DatabaseContext context)
    {
        foreach (var warehouse in context.Api_Keys)
        {
            if (VerifyAPIKey(api_key, warehouse.ApiKey)) { return warehouse.Warehouse_Id; }
        }
        return 0;
    }

    public static bool VerifyAPIKey(string api_key, string hashed_api_key)
    {
        return BCrypt.Net.BCrypt.Verify(api_key, hashed_api_key);
    }
}
