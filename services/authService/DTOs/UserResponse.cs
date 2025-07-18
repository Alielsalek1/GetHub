using System.Text.Json.Serialization;
using authService.enums;

namespace authService.Dtos;
public class UserResponse
{
    public required Guid id { get; set; }
    public required string username { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Roles role { get; set; }
}