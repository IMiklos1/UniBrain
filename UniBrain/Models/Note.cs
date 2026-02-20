namespace UniBrain.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; } = ""; // Markdown szöveg
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Kapcsolódhat tantárgyhoz (általános jegyzet)
        public int? SubjectId { get; set; }

        // VAGY kapcsolódhat konkrét órához (aznapi jegyzet)
        public int? ClassSessionId { get; set; }
    }
}
