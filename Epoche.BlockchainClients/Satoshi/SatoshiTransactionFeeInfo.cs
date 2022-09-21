namespace Epoche.BlockchainClients.Satoshi;

public class SatoshiTransactionFeeInfo
{
    public readonly SatoshiTransaction SatoshiTransaction;
    public readonly decimal Fee;
    decimal? feePerByte;
    public decimal FeePerByte => feePerByte ??= Fee / SatoshiTransaction.VirtualSize;

    public SatoshiTransactionFeeInfo(SatoshiTransaction satoshiTransaction, decimal fee)
    {
        SatoshiTransaction = satoshiTransaction ?? throw new ArgumentNullException(nameof(satoshiTransaction));
        Fee = fee;
    }
}
