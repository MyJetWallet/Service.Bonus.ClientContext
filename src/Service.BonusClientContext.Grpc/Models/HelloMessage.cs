using System.Runtime.Serialization;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusClientContext.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}