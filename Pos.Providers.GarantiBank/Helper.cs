using System.Security.Cryptography;
using System.Text;

namespace Pos.Providers.GarantiBank;

public static class Helper
{
    public static string Sha1(string text)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var inputBytes = SHA1.HashData(Encoding.GetEncoding("ISO-8859-9").GetBytes(text));
        return BytesToHex(inputBytes).ToUpper();
    }

    public static string Sha512(string text)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var inputBytes = SHA512.HashData(Encoding.GetEncoding("ISO-8859-9").GetBytes(text));
        return BytesToHex(inputBytes).ToUpper();
    }

    public static string ThreeDHashData(
        string terminalID, string orderID, string amount, int currencyCode,
        string successUrl, string errorUrl, string type, string installmentCount,
        string storeKey, string hashedPassword)
    {
        return Sha512(terminalID + orderID + amount + currencyCode + successUrl + errorUrl
                      + type + installmentCount + storeKey + hashedPassword).ToUpper();
    }

    public static string IsRequireZero(string id, int complete)
    {
        var tmp = id.Trim();
        while (tmp.Length < complete)
            tmp = "0" + tmp;
        return tmp;
    }

    public static string GetHashData(
        string provisionPassword, string terminalId, string orderId,
        int installmentCount, string storeKey, ulong amount,
        int currencyCode, string successUrl, string type, string errorUrl)
    {
        var hashedPassword = Sha1(provisionPassword + "0" + terminalId);
        return Sha512(terminalId + orderId + amount + currencyCode + successUrl + errorUrl
                      + type + installmentCount + storeKey + hashedPassword).ToUpper();
    }

    private static string BytesToHex(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
