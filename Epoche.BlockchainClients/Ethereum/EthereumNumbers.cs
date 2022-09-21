using System.Globalization;
using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;
public static class EthereumNumbers
{
    public static string Trim0x(this string s) => s.StartsWith("0x") ? s[2..] : s;
    public static string ToUint256String(long num) => num.ToString("x").PadLeft(64, '0')[..64];
    public static string ToUint256String(BigInteger num) => num.ToString("x").PadLeft(64, '0')[..64];
    public static string[] SplitReturnValues(string returnValue) => TrySplitReturnValues(returnValue) ?? throw new InvalidOperationException("Return value length % 64 != 0");
    public static string[]? TrySplitReturnValues(string returnValue)
    {
        returnValue = returnValue.Trim0x();
        return returnValue.Length % 64 != 0 ?
            null :
            Enumerable.Range(0, returnValue.Length / 64)
                .Select(x => returnValue.Substring(x * 64, 64))
                .ToArray();
    }

    public static string ToUint160String(string uint256) =>
        uint256.Length != 64 ?
        throw new InvalidOperationException("uint256 length != 64") :
        uint256[24..];
    public static string Uint160To256String(string uint160) =>
        uint160.Length != 40 ?
        throw new InvalidOperationException("uint256 length != 40") :
        "000000000000000000000000" + uint160;
    public static BigInteger ToBigInteger(string uintString) => BigInteger.Parse("0" + uintString, NumberStyles.HexNumber);
    public static long ToInt64(string uintString, bool throwIfOverflow)
    {
        var i = ToBigInteger(uintString);
        if (i > long.MaxValue)
        {
            if (throwIfOverflow)
            {
                throw new OverflowException($"{i} does not fit into int64");
            }
            return long.MaxValue;
        }
        return (long)i;
    }
}
