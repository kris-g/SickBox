namespace KrisG.SickBox.Core.Configuration.ShowNameProvider
{
    public interface IShowNameProviderConfig
    {
         IShowNameOverride[] Overrides { get; }
    }
}