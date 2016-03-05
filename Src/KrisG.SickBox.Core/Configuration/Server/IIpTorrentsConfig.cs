using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.Server
{
    public interface IIpTorrentsConfig : IUrlConfig
    {
        [Required]
        string Username { get; }

        [Required]
        string Password { get; }
    }
}