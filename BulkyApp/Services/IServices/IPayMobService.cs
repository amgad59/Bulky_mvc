namespace BulkyApp.Services.IServices
{
    public interface IPayMobService
    {
        Task<T> FirstStep<T>();
        Task<T> SecondStep<T>(string token);
        Task<T> ThirdStep<T>(string token,int id);
    }
}
