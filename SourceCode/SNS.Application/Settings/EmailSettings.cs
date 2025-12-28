namespace SNS.Application.Settings
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
    }
}
