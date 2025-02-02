namespace DecisionBackend.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(string username);
    }
}
