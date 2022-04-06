using System;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class RegistrationEvent
    {
        [DataMember(Order = 1)] public bool UserRegistered { get; set; } = true;
        [DataMember(Order = 2)] public DateTime RegistrationDate { get; set; }
    }
}