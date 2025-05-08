namespace AtonTask.Exceptions
{
    public class UserRevokedException : InvalidOperationException
    {
        public UserRevokedException(string login) 
            : base($"User '{login}' is inactive (revoked)") {}
    }
}