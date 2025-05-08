namespace AtonTask.Models.OutputModels
{
    public record UserResponseDto
    {
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? RevokedOn { get; set; }
    }
}