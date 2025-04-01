namespace AutenticaAPI;


    public enum EnumEmailType
    {
       Welcome,
       RecoveryPassord
    }
    public class Email
    {
        public required string[] To { get; set; }
        public required  EnumEmailType EmailType { get; set; }

        public required string Name { get; set; }

}

