namespace SEVEN.MissionControl.API.Client
{
    public class APIClient
    {
        private readonly string _baseUrl;

        public APIClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
    }
}
