using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class DepositEvent
    {
        [DataMember(Order = 1)]public string DepositId { get; set; }
        [DataMember(Order = 2)]public string AssetId { get; set; }
        [DataMember(Order = 3)]public decimal Amount { get; set; }
    }
}