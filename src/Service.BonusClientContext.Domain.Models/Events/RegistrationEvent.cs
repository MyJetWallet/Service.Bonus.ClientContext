using System.Runtime.Serialization;

namespace Service.BonusClientContext.Domain.Models.Events
{
    [DataContract]
    public class RegistrationEvent
    {
        [DataMember(Order = 1)] public bool UserRegistered { get; set; } = true;
    }
}