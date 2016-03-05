using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.TorrentCompleteNotifier
{
    public interface IFilePollTorrentCompleteNotifierConfig
    {
        string[] CompleteFilePossibleDestinationPaths { get; }

        [DefaultValue(5)]
        int FileArrivePollIntervalSeconds { get; }

        [DefaultValue(30)]
        int FileArrivePollTotalTimeMinutes { get; }

        [DefaultValue(5)]
        int FileGrowingPollIntervalSeconds { get; }

        [DefaultValue(30)]
        int FileGrowingPollTotalTimeMinutes { get; }
    }
}