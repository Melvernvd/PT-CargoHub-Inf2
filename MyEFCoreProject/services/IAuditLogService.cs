public static class AuditLogService
{
    private static readonly string _actionLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "audit_action_log.txt");
    private static readonly string _apikeyLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "audit_apikey_log.txt");

    public static async Task LogActionAsync(string action, string description, string apiKey)
    {
        var actionLogEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | Action: {action} | Description: {description} | ApiKey: {apiKey}\n";

        var actionLogDirectory = Path.GetDirectoryName(_actionLogFilePath);
        if (!Directory.Exists(actionLogDirectory))
        {
            Directory.CreateDirectory(actionLogDirectory);
        }

        await File.AppendAllTextAsync(_actionLogFilePath, actionLogEntry);
    }

    public static async Task LogAPIKeyAsync(string action, string description, string apiKey)
    {
        var apikeyLogEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | Action: {action} | Description: {description} | ApiKey: {apiKey}\n";

        var apikeyLogDirectory = Path.GetDirectoryName(_apikeyLogFilePath);
        if (!Directory.Exists(apikeyLogDirectory))
        {
            Directory.CreateDirectory(apikeyLogDirectory);
        }

        await File.AppendAllTextAsync(_apikeyLogFilePath, apikeyLogEntry);
    }
}
