namespace EmpireApp.Services.IServices
{
    public interface IPayMobService
    {
        Task<T> Refund<T>(int transactionId, int orderTotal);
        Task<T> FirstPayMobStep<T>();
        Task<T> SecondPayMobStep<T>(Dictionary<string, object> payload);
        Task<T> ThirdPayMobStep<T>(Dictionary<string,object> payload);
        Task<string> PayMobSetup(Dictionary<string, object> FirstStepPayload
            , Dictionary<string, object> SecondStepPayload);
    }
}
