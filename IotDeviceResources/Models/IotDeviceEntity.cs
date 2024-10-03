using SQLite;
using System.ComponentModel.DataAnnotations;

namespace IotDeviceResources.Models;

public class IotDeviceEntity
{
    [Key]
    [PrimaryKey]
    public string Id { get; set; } = null!;
    public string? Type { get; set; }
    public string? ConnectionString { get; set; }
}
