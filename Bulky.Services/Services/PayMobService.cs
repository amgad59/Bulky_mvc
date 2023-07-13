using Empire.Models.API;
using Empire.Models.API.DTO;
using Empire.Utilities;
using EmpireApp.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Empire.Utilities.SD;

namespace EmpireApp.Services
{
    public class PayMobService : BaseService,IPayMobService
    {
        private string token;
        private PayMobSettings _payMobSettings;
        public PayMobService(IHttpClientFactory httpClient, IOptions<PayMobSettings> payMobSettings) : base(httpClient)
        {
            _payMobSettings = payMobSettings.Value;
        }
        public async Task<string> PayMobSetup(Dictionary<string,object> FirstStepPayload
            ,Dictionary<string, object> SecondStepPayload)
        {
            var x = await FirstPayMobStep<PayMobFirstAPIResponse>();
            FirstStepPayload["auth_token"] = x.token;
            var y = await SecondPayMobStep<PayMobSecondAPIResponse>(x.token, FirstStepPayload);
            SecondStepPayload["auth_token"] = x.token;
            SecondStepPayload["order_id"] = y.id;
            var z = await ThirdPayMobStep<PayMobThirdAPIResponse>(x.token, y.id, SecondStepPayload);
            return z.token;
        }
        public async Task<T> FirstPayMobStep<T>()
        {
            return await SendAsync<T>(new APIRequest()
            {
                Url = _payMobSettings.FirstStepUrl,
                ApiType = ApiType.POST,
                data = new Dictionary<string, object> {
                    {
                        "api_key", _payMobSettings.API_Key
                    }
                }
            });
        }
        public async Task<T> SecondPayMobStep<T>(string token, Dictionary<string, object> Payload)
        {
            return await SendAsync<T>(new APIRequest()
            {
                Url = _payMobSettings.SecondStepUrl,
                ApiType = ApiType.POST,
                data = Payload
            });
        }
        public async Task<T> ThirdPayMobStep<T>(string token, int id,Dictionary<string, object> Payload)
        {
            return await SendAsync<T>(new APIRequest()
            {
                Url = _payMobSettings.ThirdStepUrl,
                ApiType = ApiType.POST,
                data = Payload
            });
        }
    }
}
