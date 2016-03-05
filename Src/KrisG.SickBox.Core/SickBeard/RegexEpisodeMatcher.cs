using System;
using System.Linq;
using System.Text.RegularExpressions;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using log4net;

namespace KrisG.SickBox.Core.SickBeard
{
    public class RegexEpisodeMatcher : IEpisodeMatcher
    {
        private readonly IShowNameProvider _showNameProvider;
        private readonly ILog _log;

        public RegexEpisodeMatcher(IShowNameProvider showNameProvider, ILog log)
        {
            _showNameProvider = showNameProvider;
            _log = log;
        }

        public bool IsMatch(string input, IEpisode episode)
        {
            var cleanedShowName = _showNameProvider.Get(episode);

            // maintain the spaces for replacing with .? later
            // but still escape everything else for regex
            var showNameWords = cleanedShowName.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
            var regexEscapedWords = showNameWords.Select(Regex.Escape);
            var regexShowName = string.Join(".?", regexEscapedWords);
            
            var regexPattern = string.Format("^{0}.?[sS]{1:00}[eE]{2:00}", regexShowName, episode.SeasonNumber, episode.EpisodeNumber);
            var isMatch = Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);

            _log.DebugFormat("Episode match {0} [Input: {1}, Regex: {2}]", isMatch ? "passed" : "failed", input, regexPattern);

            return isMatch;
        }
    }
}