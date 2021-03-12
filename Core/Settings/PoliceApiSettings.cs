namespace Core.Settings
{
    public class PoliceApiSettings
    {
        public PoliceApiSettings()
        {
            PollingIntervalMinutes = 10;
        }

        public string PoliceApiUrl { get; set; }
        public int PollingIntervalMinutes { get; set; }
    }
}
