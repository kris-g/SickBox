namespace KrisG.Utility.Interfaces.Configuration
{
    public interface IConfigurable<T>
    {
         T Config { get; }
    }
}