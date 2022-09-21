using System.Globalization;

namespace Epoche.BlockchainClients.Ethereum;

public class EthereumHexInt32Converter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only string can be converted with this converter");
        }

        return FromString(reader.GetString());
    }

    public static int FromString(string? s)
    {
        if (s is null ||
            s.Length < 3 ||
            s[0] != '0' ||
            (s[1] != 'x' && s[1] != 'X'))
        {
            throw new FormatException("Ethereum hex must start with 0x#");
        }

        return checked((int)uint.Parse(s[2..], NumberStyles.HexNumber));
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        if (value < 0)
        {
            throw new InvalidOperationException();
        }
        writer.WriteStringValue("0x" + value.ToString("x"));
    }
}
