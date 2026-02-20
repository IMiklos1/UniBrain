namespace UniBrain.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public required string FileName { get; set; } // Eredeti név (pl. "tabla_foto.jpg")
        public required string StoredFileName { get; set; } // A szerveren lévő név (egyedi)
        public string ContentType { get; set; } = "application/octet-stream"; // Mime type

        public int ClassSessionId { get; set; } // Melyik órához tartozik
    }
}
