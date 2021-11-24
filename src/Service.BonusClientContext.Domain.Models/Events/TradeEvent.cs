using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class TradeEvent
    {
        [DataMember(Order = 1)]public string TradeId { get; set; }
        [DataMember(Order = 2)]public string FromAssetId { get; set; }
        [DataMember(Order = 3)]public decimal FromAmount { get; set; }
        [DataMember(Order = 4)]public string ToAssetId { get; set; }
        [DataMember(Order = 5)]public decimal ToAmount { get; set; }
    }
}