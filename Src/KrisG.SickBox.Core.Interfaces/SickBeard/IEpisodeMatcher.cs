namespace KrisG.SickBox.Core.Interfaces.SickBeard
{
    public interface IEpisodeMatcher
    {
        bool IsMatch(string input, IEpisode episode);
    }
}