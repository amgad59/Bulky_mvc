using Bulky.Models.API;
using BulkyApp.Services.IServices;
using Microsoft.Extensions.Configuration;
using static Bulky.Utilities.SD;

namespace BulkyApp.Services
{
    public class PayMobService : BaseService,IPayMobService
    {
        private string testURL = "https://api.publicapis.org/entries";
        public PayMobService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            testURL = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> testCall<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = testURL,
                ApiType = ApiType.GET
            });
        }
    }
}
