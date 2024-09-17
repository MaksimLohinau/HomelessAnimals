using System.Text.Json.Serialization;

namespace HomelessAnimals.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ScopeLevel
    {
        System,
        Animal,
        Volounteer,
    }
}
