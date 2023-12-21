using System.Text.Json.Serialization;

namespace MISBack.Data.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Conclusion
{
    Disease,
    Recovery,
    Death
}