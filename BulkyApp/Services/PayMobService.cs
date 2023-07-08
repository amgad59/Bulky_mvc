using Bulky.Models.API;
using BulkyApp.Services.IServices;
using Microsoft.Extensions.Configuration;
using static Bulky.Utilities.SD;

namespace BulkyApp.Services
{
    public class PayMobService : BaseService,IPayMobService
    {
        private string FirstStepUrl;
        private string SecondStepUrl;
        private string ThirdStepUrl;
        private string API_Key;
        private string token;
        public PayMobService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            FirstStepUrl = configuration.GetValue<string>("ServiceUrls:PayMobFirst");
            SecondStepUrl = configuration.GetValue<string>("ServiceUrls:PayMobSecond");
            ThirdStepUrl = configuration.GetValue<string>("ServiceUrls:PayMobThird");
            API_Key = configuration.GetValue<string>("PayMob:API_Key");
        }

        public Task<T> FirstStep<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = FirstStepUrl,
                ApiType = ApiType.POST,
                data = new Dictionary<string, object> {
                    {
                        "api_key", API_Key
                    }
                }
            });
        }
        public class Item
        {
            public string name;
            public int amount_cents;
            public string description;
            public int quantity;
        }
        public Task<T> SecondStep<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = SecondStepUrl,
                ApiType = ApiType.POST,
                data = new Dictionary<string, object> {
                    {
                    "auth_token",  token
                    },
                    {"delivery_needed", "false"},
                    { "amount_cents", "2000" },
                    { "currency", "EGP" },
                    { "items", new Item[]{ new Item {name= "ASC1515",amount_cents= 500000,description= "Smart Watch",quantity=1 } } }
            }
            });
        }
        public Task<T> ThirdStep<T>(string token, int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = ThirdStepUrl,
                ApiType = ApiType.POST,
                data = new Dictionary<string, object> {

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
            }
            });
        }
    }
}
