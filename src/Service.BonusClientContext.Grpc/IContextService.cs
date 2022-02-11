using System.ServiceModel;
using System.Threading.Tasks;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Grpc.Models;

namespace Service.BonusClientContext.Grpc
{
    [ServiceContract]
    public interface IContextService
    {
        [OperationContract]
        Task<ClientContext> GetContextByClientId(GetContextRequest request);
        
        [OperationContract]
        Task<AllContextsResponse> GetAllContexts();
     
        [OperationContract]
        Task StartCheckForAll();
    }
}