namespace MySecureBackend.WebApi.Models
{
    public class Object2D
    {
        public Guid? Id { get; set; }
        public int? PosX { get; set; }
        public int? PosY { get; set; }
        public int? TileId { get; set; }
        public string? Layer { get; set; } 
        public Guid? EnvironmentId { get; set; }
        
        public Environment2D? environment2D { get; set; }
    } 
}

