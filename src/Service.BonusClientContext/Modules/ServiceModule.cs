﻿using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Domain.ServiceBus;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.Bitgo.DepositDetector.Domain.Models;
using Service.BonusClientContext.Domain.Models;
using Service.BonusClientContext.Jobs;
using Service.ClientProfile.Domain.Models;

namespace Service.BonusClientContext.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var spotServiceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);

            var queueName = "Service.Bonus.ClientContext";
            builder.RegisterClientRegistrationSubscriber(spotServiceBusClient, queueName, TopicQueueType.PermanentWithSingleConnection, false);
            builder.RegisterMyServiceBusSubscriberSingle<ClientProfileUpdateMessage>(spotServiceBusClient,
                ClientProfileUpdateMessage.TopicName, queueName, TopicQueueType.PermanentWithSingleConnection);
            builder.RegisterMyServiceBusSubscriberSingle<Deposit>(spotServiceBusClient, Deposit.TopicName, queueName,
                TopicQueueType.PermanentWithSingleConnection);

            builder.RegisterMyServiceBusPublisher<ContextUpdate>(spotServiceBusClient, ContextUpdate.TopicName, true);


            builder.RegisterType<DepositUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<ProfileUpdateJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<RegistrationUpdateJob>().AsSelf().AutoActivate().SingleInstance();
        }
    }
}