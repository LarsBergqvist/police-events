using System.Collections.Generic;

namespace Core.Models
{
    public class PoliceEventsResult
    {
        public long TotalPages { get; set; }
        public long Page { get; set; }
        public IList<PoliceEvent> Events { get; set; }
    }
}
