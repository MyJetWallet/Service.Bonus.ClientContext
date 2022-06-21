using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Domain.ServiceBus;
using MyJetWallet.Sdk.Authorization.ServiceBus;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.Bitgo.DepositDetector.Domain.Models;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Jobs;
using Service.BonusClientContext.Services;
using Service.ClientProfile.Client;
using Service.ClientProfile.Domain.Models;
using Service.KYC.Domain.Models.Messages;
using Service.Liquidity.Converter.Domain.Models;
using Service.PersonalData.Client;
using Service.Registration.Domain.Models;

namespace Service.BonusClientContext.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var spotServiceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);

            var queueName = "Service.Bonus.ClientContext";
            builder.RegisterMyServiceBusSubscriberSingle<ClientProfileUpdateMessage>(spotServiceBusClient,
                ClientProfileUpdateMessage.TopicName, queueName, TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusSubscriberSingle<Deposit>(spotServiceBusClient, Deposit.TopicName, queueName,
                TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusSubscriberSingle<ClientRegisterMessage>(spotServiceBusClient, ClientRegisterMessage.TopicName, queueName,
                TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusSubscriberSingle<SwapMessage>(spotServiceBusClient, SwapMessage.TopicName, queueName,
                TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusSubscriberSingle<KycProfileUpdatedMessage>(spotServiceBusClient,
                KycProfileUpdatedMessage.TopicName, queueName, TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusSubscriberSingle<SessionAuditEvent>(spotServiceBusClient,
                SessionAuditEvent.TopicName, queueName, TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusPublisher<ContextUpdate>(spotServiceBusClient, ContextUpdate.TopicName, true);

            builder.RegisterPersonalDataClient(Program.Settings.PersonalDataGrpcServiceUrl);
            builder.RegisterClientProfileClientWithoutCache(Program.Settings.ClientProfileGrpcServiceUrl);
            
            builder.RegisterType<DepositUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            //builder.RegisterType<ProfileUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<RegistrationUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<TradeUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<KycUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<LoginUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<ManualUpdateService>().AsSelf().SingleInstance();


        }
    }
}