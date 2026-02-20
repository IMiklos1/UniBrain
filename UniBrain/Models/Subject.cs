using System.Text.Json.Serialization;

namespace UniBrain.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Name { get; set; } // Pl. "Beágyazott rendszerek"
        public required string Code { get; set; } // Pl. "GEVAU160-ML"
        public string? Teacher { get; set; }      // Pl. "Dr. Vásárhelyi József"

        // Navigációs propertyk
        [JsonIgnore] // Hogy ne legyen körkörös hivatkozás a JSON-ben
        public List<ClassSession> Sessions { get; set; } = [];
        public List<Note> Notes { get; set; } = [];
    }
}
