using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ApiKeyService
{
    public static string GenerateApiKey()
    {
        return Guid.NewGuid().ToString(); 
    }

    public static string HashApiKey(string apiKey)
    {
        return BCrypt.Net.BCrypt.HashPassword(apiKey);
    }

    public static async Task<string> RequestGenerateAsync(int warehouse_id, DatabaseContext context)
    {
        await CreateAndSaveApiKeyAsync("dashboard", warehouse_id, context);
        await CreateAndSaveApiKeyAsync("scanner", warehouse_id, context);
        return "";
    }

    public static async Task<string> CreateAndSaveApiKeyAsync(string appName, int warehouse_id, DatabaseContext context)
    {
        var rawApiKey = GenerateApiKey();
        var encryptApiKey = HashApiKey(rawApiKey);

        var newApiKey = new Api_Key
        {
            ApiKey = encryptApiKey,
            Warehouse_Id = warehouse_id,
            App = appName
        };

        context.Api_Keys.Add(newApiKey);
        await context.SaveChangesAsync();

        Console.WriteLine($"Warehouse '{warehouse_id}' raw api key: {rawApiKey}");

        return $"{rawApiKey}, {warehouse_id}, {appName}";
    }
}