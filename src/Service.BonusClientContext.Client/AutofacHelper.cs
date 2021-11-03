using Autofac;
using Service.BonusClientContext.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.BonusClientContext.Client
{
    public static class AutofacHelper
    {
        public static void RegisterBonusClientContextClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BonusClientContextClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
