namespace AtonTask.Models.InputModels
{
    public record UserUpdateDto
    {
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Gender { get; set; }
    }
}