using System.Runtime.Serialization;
using Service.BonusClientContext.Domain.Models.Events;

namespace Service.BonusClientContext.Domain.Models
{
    [DataContract]
    public class ContextUpdate
    {
        public const string TopicName = "jet-wallet-bonus-context-update"; 
        
        [DataMember(Order = 1)] public EventType EventType { get; set; }
        [DataMember(Order = 2)]public string ClientId { get; set; }
        [DataMember(Order = 3)]public ClientContext Context { get; set; }
        
        [DataMember(Order = 4)] public KYCEvent KycEvent { get; set; }
        [DataMember(Order = 5)] public DepositEvent DepositEvent { get; set; }
        [DataMember(Order = 6)] public RegistrationEvent RegistrationEvent { get; set; }
        [DataMember(Order = 7)] public ReferrerAddedEvent ReferrerAddedEvent { get; set; }

    }
}