using KrisG.SickBox.Core.Configuration.TorrentPostProcessor;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;
using PushBulletSharp.Core;
using PushBulletSharp.Core.Models.Requests;
using PushBulletSharp.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrisG.Utility.Extensions;

namespace KrisG.SickBox.Core.Torrent.PostProcessor
{
    [ServiceImplementation("PushbulletNotification")]
    public class PushbulletNotificationTorrentPostProcessor : ITorrentPostProcessor, IConfigurable<IPushbulletNotificationTorrentPostProcessorConfig>
    {
        private readonly ILog _log;

        public IPushbulletNotificationTorrentPostProcessorConfig Config { get; private set; }

        public PushbulletNotificationTorrentPostProcessor(ILog log)
        {
            _log = log;
        }

        public void PostProcess(IEnumerable<TorrentDownloadResult> torrents)
        {
            var torrentsArray = torrents.ToArray();

            try
            {
                var client = new PushBulletClient(Config.AccessToken);

                var currentUserInformation = client.CurrentUsersInformation();

                if (currentUserInformation != null)
                {
                    var request = new PushNoteRequest
                    {
                        Email = Config.AccountEmailAddress,
                        Title = string.Format("SickBeard Downloader: {0} Items Downloaded", torrentsArray.Length),
                        Body = torrentsArray.Select(x => x.DownloadFileName).JoinStrings(Environment.NewLine)
                    };

                    Task<PushResponse> response = client.PushNote(request);
                    response.Wait();
                    _log.InfoFormat("Pushbullet notification sent to {0}", Config.AccountEmailAddress);
                }

            }
            catch (Exception ex)
            {
                _log.Error("Failed to send Pushbullet notification", ex);
            }
        }
    }
}