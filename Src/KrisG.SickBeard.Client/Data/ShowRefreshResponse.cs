namespace KrisG.SickBeard.Client.Data
{
    public class ShowRefreshResponse
    {
        public string Result { get; private set; }

        public ShowRefreshResponse(string result)
        {
            Result = result;
        }
    }
}