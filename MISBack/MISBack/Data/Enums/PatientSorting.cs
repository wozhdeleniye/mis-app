using System.Text.Json.Serialization;

namespace MISBack.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PatientSorting
    {
        NameAsc,
        NameDesc,
        CreateAsc,
        CreateDesc,
        InspectionAsc,
        InspectionDesc
    }
}
