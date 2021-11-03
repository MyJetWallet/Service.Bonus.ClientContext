using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class ReferrerAddedEvent
    {
        [DataMember(Order = 1)] public string ReferrerClientId { get; set; }
    }
}