using System.Globalization;

namespace Pos.Providers.GarantiBank;

public static class NumberHelpers
{
    /// <summary>
    /// Tutarı Garanti Bankası formatına çevirir (kuruş cinsinden, ondalıksız).
    /// Örnek: 10.50 → "1050"
    /// </summary>
    public static string ToString(decimal value) =>
        ((long)Math.Round(value, MidpointRounding.AwayFromZero)).ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Bankadan dönen tutar string'ini decimal'e çevirir.
    /// </summary>
    public static decimal ParseMoney(string value) =>
        decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
}

