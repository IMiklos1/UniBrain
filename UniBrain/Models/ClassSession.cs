namespace UniBrain.Models
{
    public class ClassSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } // Dátum + Start egyesítve
        public DateTime EndTime { get; set; }   // Dátum + End egyesítve
        public string? Room { get; set; }       // Pl. "In/214"
        public string? Type { get; set; }       // "normal" vagy "potlo"

        // Foreign Key
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
