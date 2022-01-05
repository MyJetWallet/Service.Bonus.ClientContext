using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class KYCEvent
    {
        [DataMember(Order = 1)] public bool KycDepositPassed { get; set; }
        [DataMember(Order = 2)] public bool KycTradePassed { get; set; }
        [DataMember(Order = 3)] public bool KycWithdrawalPassed { get; set; }
    }
}