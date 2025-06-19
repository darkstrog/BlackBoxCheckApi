namespace BlackBoxCheckApi.Models.Profiles
{
    public class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }

    }
}
