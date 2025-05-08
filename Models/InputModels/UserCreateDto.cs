namespace AtonTask.Models.InputModels
{
    public record UserCreateDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool IsAdmin { get; set; }
    }
}