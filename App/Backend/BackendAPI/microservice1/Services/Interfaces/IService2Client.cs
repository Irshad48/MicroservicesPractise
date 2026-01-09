namespace microservice1.Services.Interfaces
{
    public interface IService2Client
    {
        Task<string> GetMessageFromService2Async();
    }
}
