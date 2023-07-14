using Empire.Models.API;
using Empire.Models.API.DTO;
using Empire.Models.ViewModels;
using Empire.Utilities;
using EmpireApp.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Empire.Utilities.SD;

namespace EmpireApp.Services
{

    // TODO : complete the refund by adding the token in the OrderHeader table at the Models 

    public class PayMobService : BaseService,IPayMobService
    {
        private string token;
        private PayMobSettings _payMobSettings;
        public PayMobService(IHttpClientFactory httpClient, IOptions<PayMobSettings> payMobSettings) : base(httpClient)
        {
            _payMobSettings = payMobSettings.Value;
        }
        public async Task<T> Refund<T>(int transactionId,int orderTotal)
        {
            var response = await FirstPayMobStep<PayMobFirstAPIResponse>();
            Dictionary<string, object> Payload = new Dictionary<string, object> {
                    {
                    "auth_token", response.token
                    },
                    {"transaction_id", transactionId},
                    {"amount_cents", orderTotal*100}
            };
            return await SendAsync<T>(new APIRequest()
            {
                Url = "https://accept.paymob.com/api/acceptance/void_refund/refund",
                ApiType = ApiType.POST,
                data = Payload
            });
        }
        public async Task<string> PayMobSetup(Dictionary<string,object> FirstStepPayload
            ,Dictionary<string, object> SecondStepPayload)
        {
            var x = await FirstPayMobStep<PayMobFirstAPIResponse>();
            FirstStepPayload["auth_token"] = x.token;
            var y = await SecondPayMobStep<PayMobSecondAPIResponse>(FirstStepPayload);
            SecondStepPayload["auth_token"] = x.token;
            SecondStepPayload["order_id"] = y.id;
            var z = await ThirdPayMobStep<PayMobThirdAPIResponse>(SecondStepPayload);
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
        public async Task<T> SecondPayMobStep<T>(Dictionary<string, object> Payload)
        {
            return await SendAsync<T>(new APIRequest()
            {
                Url = _payMobSettings.SecondStepUrl,
                ApiType = ApiType.POST,
                data = Payload
            });
        }
        public async Task<T> ThirdPayMobStep<T>(Dictionary<string, object> Payload)
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
