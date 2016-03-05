using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.TorrentPostProcessor
{
    public interface IPushbulletNotificationTorrentPostProcessorConfig
    {
        [Required]
        string AccountEmailAddress { get; }

        [Required]
        string AccessToken { get; }
    }
}