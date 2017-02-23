namespace KrisG.IpTorrents.Client.Interfaces
{
    public interface IProxyConfig
    {
        ProxyServerType Type { get; }
        string ServerIpAddress { get; }        
        int ServerPort { get; }
    }
}