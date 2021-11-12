using System.Runtime.Serialization;

namespace Service.BonusClientContext.Grpc.Models
{
    [DataContract]
    public class GetContextRequest
    {
        [DataMember(Order = 1)]
        public string ClientId { get; set; }
    }
}