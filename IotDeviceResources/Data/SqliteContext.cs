using IotDeviceResources.Factories;
using IotDeviceResources.Models;
using Microsoft.Extensions.Logging;
using SQLite;

namespace IotDeviceResources.Data;

public class SqliteContext : IDatabaseContext
{
    private readonly ILogger<SqliteContext> _logger;
    private readonly SQLiteAsyncConnection? _context;

    public SqliteContext(ILogger<SqliteContext> logger, Func<string> directoryPath, string databaseName = "database.db3")
    {
        _logger = logger;

        try
        {
            var databasePath = Path.Combine(directoryPath(), databaseName);
            if (string.IsNullOrWhiteSpace(databasePath))
                throw new ArgumentException("The database path cannot be null or empty.");

            _context = new SQLiteAsyncConnection(databasePath);

            CreateTablesAsync().ConfigureAwait(false);
        }
        catch { _logger.LogError("An error occurred while creating the database connection."); }
    }

    public async Task CreateTablesAsync()
    {
        try
        {
            if(_context == null)
                throw new ArgumentException("The database has not been initialized.");

            await _context.CreateTableAsync<IotDeviceSettings>();

            _logger.LogWarning("Database tables were created successfully.");
        }
        catch { _logger.LogError("An error occurred while creating the database tables."); }
    }

    public async Task<ResponseResult<IotDeviceSettings>> GetSettingsAsync()
    {
        try
        {
            var iotDeviceSettings = (await _context!.Table<IotDeviceSettings>().ToListAsync()).SingleOrDefault();
            if(iotDeviceSettings != null)
                return ResponseResultFactory<IotDeviceSettings>.Succeeded($"Iot Device Settings for Device: {iotDeviceSettings.Id} were successfully collected.", iotDeviceSettings);

            return ResponseResultFactory<IotDeviceSettings>.Failed("No iot device settings were found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the iot device settings.");
            return ResponseResultFactory<IotDeviceSettings>.Failed("An error occurred while getting the iot device settings.");
        }
    }

    public async Task<ResponseResult> ResetSettingsAsync()
    {
        try
        {
            await _context!.DeleteAllAsync<IotDeviceSettings>();
            var iotDeviceSettings = await _context.Table<IotDeviceSettings>().ToListAsync();

            if(iotDeviceSettings.Count == 0)
                return ResponseResultFactory.Succeeded("Iot device settings were reset successfully.");

            _logger.LogError("Iot device settings failed to reset.");
            return ResponseResultFactory.Failed("Iot device settings failed to reset.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while resetting the iot device settings.");
            return ResponseResultFactory.Failed("An error occurred while resetting the iot device settings.");
        }
    }

    public async Task<ResponseResult> SaveSettingsAsync(IotDeviceSettings iotDeviceSettings)
    {
        try
        {
            if (!string.IsNullOrEmpty(iotDeviceSettings.Id))
            {
                var response = await GetSettingsAsync();

                if (response.Content != null)
                {
                    response.Content.ConnectionString = iotDeviceSettings.ConnectionString; 
                    response.Content.Type = iotDeviceSettings.Type;

                    await _context!.UpdateAsync(iotDeviceSettings);
                    return ResponseResultFactory.Succeeded("Iot device settings were updated successfully.");
                }
                else
                {
                    await _context!.InsertAsync(iotDeviceSettings);
                    return ResponseResultFactory.Succeeded("Iot device settings were inserted successfully.");
                }
            }

            _logger.LogError("Failed to save iot device settings: ID is null or empty.");
            return ResponseResultFactory.Failed("Failed to save iot device settings: ID is null or empty.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while saving the iot device settings.");
            return ResponseResultFactory.Failed("An error occurred while saving the iot device settings.");
        }
    }
}
