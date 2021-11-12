using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.BonusClientContext.Grpc;

namespace Service.BonusClientContext.Client
{
    [UsedImplicitly]
    public class BonusClientContextClientFactory: MyGrpcClientFactory
    {
        public BonusClientContextClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IContextService GetContextService() => CreateGrpcService<IContextService>();
    }
}
