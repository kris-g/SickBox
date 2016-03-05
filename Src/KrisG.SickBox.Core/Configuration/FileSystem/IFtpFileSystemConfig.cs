using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.FileSystem
{
    public interface IFtpFileSystemConfig
    {
        [Required]
        string Host { get; }

        [DefaultValue(21)]
        int Port { get; }
        
        [Required]
        string Username { get; }
        
        [Required]
        string Password { get; }
    }
}