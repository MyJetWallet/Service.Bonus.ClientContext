namespace Service.BonusClientContext.Domain.Models
{
    public enum EventType
    {
        ClientRegistered,
        KYCPassed,
        ReferrerAdded,
        DepositMade,
        TradeMade,
        WithdrawalMade,
        ManualCheckEvent
    }
}