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
        [DataMember(Order = 3)] public bool KYCDone { get; set; }
        [DataMember(Order = 4)] public bool HasReferrer { get; set; }
        [DataMember(Order = 5)] public bool HasReferrals { get; set; }
        [DataMember(Order = 6)] public string ReferrerClientId { get; set; }

    }
}