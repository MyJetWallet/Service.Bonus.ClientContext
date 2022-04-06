using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models
{
    [DataContract]
    public class ClientContext
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public DateTime LastRecord { get; set; } = DateTime.UtcNow;
        [DataMember(Order = 4)] public bool HasReferrer { get; set; }
        [DataMember(Order = 5)] public bool HasReferrals { get; set; }
        [DataMember(Order = 6)] public string ReferrerClientId { get; set; }
        [DataMember(Order = 7)] public string Country { get; set; }
        [DataMember(Order = 8)] public bool KycDepositAllowed { get; set; }
        [DataMember(Order = 9)] public bool KycTradeAllowed { get; set; }
        [DataMember(Order = 10)] public bool KycWithdrawalAllowed { get; set; }
        [DataMember(Order = 11)] public DateTime RegistrationDate { get; set; }
    }
}