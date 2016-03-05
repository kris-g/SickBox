namespace KrisG.SickBox.Core.Interfaces.SickBeard
{
    public interface IEpisode
    {
        string ShowName { get; }
        int ShowId { get; }
        int SeasonNumber { get; }
        int EpisodeNumber { get; }
    }
}