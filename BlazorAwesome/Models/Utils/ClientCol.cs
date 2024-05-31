using System.Text.Json.Serialization;

namespace Omu.BlazorAwesome.Models.Utils
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Grid client column
    /// </summary>
    public class ClientColumn
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("W")]
        public int W { get; set; }

        [JsonPropertyName("Grow")]
        public double Grow { get; set; }

        [JsonPropertyName("Mw")]
        public int Mw { get; set; }

        [JsonPropertyName("R")]
        public bool? R { get; set; }
    }
}