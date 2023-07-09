namespace BulkyApp.Services.IServices
{
    public interface IPayMobService
    {
        Task<T> FirstPayMobStep<T>();
        Task<T> SecondPayMobStep<T>(string token, Dictionary<string, object> payload);
        Task<T> ThirdPayMobStep<T>(string token,int id,Dictionary<string,object> payload);
        Task<string> PayMobSetup(Dictionary<string, object> FirstStepPayload
            , Dictionary<string, object> SecondStepPayload);
    }
}
