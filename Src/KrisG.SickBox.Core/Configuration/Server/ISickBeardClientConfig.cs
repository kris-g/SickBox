using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.Server
{
    public interface ISickBeardClientConfig : IUrlConfig
    {
        [Required]
        string ApiKey { get; }
    }
}