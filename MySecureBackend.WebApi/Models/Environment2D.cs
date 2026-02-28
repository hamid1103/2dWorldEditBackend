namespace MySecureBackend.WebApi.Models
{
    public class Environment2D
    {
        public Guid? Id { get; set; }
        
        public string Name { get; set; }
        
        public string? PreviewURL { get; set; }
        public string? UserId { get; set; }
        public ICollection<Object2D>? objects { get; set; }
    }
}