using System.Globalization;
using System.Numerics;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumHexBigIntegerConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted with this converter");
        }

        return FromString(reader.GetString());
    }

    public static BigInteger FromString(string? s)
    {
        if (s is null ||
            s.Length < 3 ||
            s[0] != '0' ||
            (s[1] != 'x' && s[1] != 'X'))
        {
            throw new FormatException("Ethereum hex must start with 0x#");
        }

        return BigInteger.Parse("00" + s[2..], NumberStyles.HexNumber); // causes leading zeroes, but which ensures no negative sign bit
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        if (value < 0)
        {
            throw new InvalidOperationException();
        }
        writer.WriteStringValue("0x" + value.ToString("x"));
    }
}
