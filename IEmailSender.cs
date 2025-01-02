namespace AutenticaAPI;

public interface IEmailSender
{
    Task<bool> EnviaEmail(Email email,string chaveValidacao);
}
