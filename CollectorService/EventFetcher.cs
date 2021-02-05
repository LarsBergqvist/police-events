using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CollectorService
{
    public class EventFetcher
    {
        public EventFetcher()
        {
        }

        public async Task<List<PoliceEvent>> Fetch()
        {
            var policeEventCollection = new List<PoliceEvent>();
            var client = new HttpClient();
            //            client.BaseAddress = new Uri("https://polisen.se/api/events");
            var res = await client.GetAsync("https://polisen.se/api/events"); //?DateTime=2021-02-03"); // &locationname=Upplands-Bro");
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                var externalData = JsonConvert.DeserializeObject<PoliceEventExternal[]>(result);
                foreach(var ext in externalData)
                {
                    var policeEvent = ext.GetPoliceEvent();
                    policeEventCollection.Add(policeEvent);
                }
            }
            return policeEventCollection;
        }
    }
}
