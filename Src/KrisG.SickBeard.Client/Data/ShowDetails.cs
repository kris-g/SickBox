namespace KrisG.SickBeard.Client.Data
{
    public class ShowDetails
    {
        public string Location { get; private set; }

        public ShowDetails(string location)
        {
            Location = location;
        }
    }
}