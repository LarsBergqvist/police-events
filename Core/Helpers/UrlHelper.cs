namespace Core.Helpers
{
    public class UrlHelper
    {
        private const string EventDetailsUrlPrefix = "https://polisen.se";
    
        public static string CompleteEventUrl(string url)
        {
            if (!string.IsNullOrEmpty(url) && !url.ToLower().Contains(EventDetailsUrlPrefix))
            {
                if (url.Length > 0 && url[0] != '/')
                {
                    url = "/" + url;                    
                }
                return string.Format($"{EventDetailsUrlPrefix}{url}");
            }
            return url;
        }
    }
}
