using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class KYCEvent
    {
        [DataMember(Order = 1)] public bool KycPassed { get; set; } = true;
    }
}