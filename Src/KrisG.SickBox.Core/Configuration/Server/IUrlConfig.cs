using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.Server
{
    public interface IUrlConfig
    {
        [Required]
        string Url { get; } 
    }
}