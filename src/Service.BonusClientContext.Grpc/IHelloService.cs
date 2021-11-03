using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusClientContext.Grpc.Models;

namespace Service.BonusClientContext.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}