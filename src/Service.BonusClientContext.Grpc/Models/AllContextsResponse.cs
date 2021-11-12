using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.BonusClientContext.Domain.Models;

namespace Service.BonusClientContext.Grpc.Models
{
    [DataContract]
    public class AllContextsResponse
    {
        [DataMember(Order = 1)]
        public List<ClientContext> Contexts { get; set; }
    }
}