namespace AutenticaAPI;

public interface IToken
{
    string CreateToken(Guid guid);
}
