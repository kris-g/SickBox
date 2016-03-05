using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.SickBox.Core.Configuration.TorrentPostProcessor;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;
using Microsoft.Practices.ObjectBuilder2;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;

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
                var client = new PushbulletClient(Config.AccessToken);

                var currentUserInformation = client.CurrentUsersInformation();

                if (currentUserInformation != null)
                {
                    var request = new PushNoteRequest
                    {
                        Email = Config.AccountEmailAddress,
                        Title = string.Format("SickBeard Downloader: {0} Items Downloaded", torrentsArray.Length),
                        Body = torrentsArray.Select(x => x.DownloadFileName).JoinStrings(Environment.NewLine)
                    };

                    PushResponse response = client.PushNote(request);
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