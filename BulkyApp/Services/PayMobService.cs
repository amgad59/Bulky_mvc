using Bulky.Models.API;
using Bulky.Models.API.DTO;
using Bulky.Utilities;
using BulkyApp.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Bulky.Utilities.SD;

namespace BulkyApp.Services
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
            /*new Dictionary<string, object> {
                    {
                    "auth_token",  token
                    },
                    {"delivery_needed", "false"},
                    { "amount_cents", "2000" },
                    { "currency", "EGP" },
                    { "items", new Item[]{ new Item {name= "ASC1515",amount_cents= 500000,description= "Smart Watch",quantity=1 } } }
            }*/
            });
        }
        public async Task<T> ThirdPayMobStep<T>(string token, int id,Dictionary<string, object> Payload)
        {
            return await SendAsync<T>(new APIRequest()
            {
                Url = _payMobSettings.ThirdStepUrl,
                ApiType = ApiType.POST,
                data = Payload
                /*new Dictionary<string, object> {

                    { "auth_token",token },
                    { "amount_cents", "100" },
                    { "expiration", 3600 },
                    { "order_id", "134740221" },
                    { "billing_data",new Dictionary<string, object>{
                        {"apartment", "803" },
                        {"email", "claudette09@exa.com" },
                        {"floor", "42"},
                        {"first_name", "Clifford"},
                        {"street", "Ethan Land"},
                        {"building", "8028"},
                        {"phone_number", "+86(8)9135210487"},
                        {"shipping_method", "PKG"},
                        {"postal_code", "01898"},
                        {"city", "Jaskolskiburgh"},
                        {"country", "CR"},
                        {"last_name", "Nicolas"},
                        { "state", "Utah"} } 
                    },
                    { "currency", "EGP"},
                    { "integration_id", 3951279}
                }*/
            });
        }
    }
}
