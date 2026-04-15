namespace Pos.Providers.GarantiBank;

/// <summary>
/// Garanti Bankası 3D Secure entegrasyonu için yapılandırma değerleri.
/// appsettings.json → "GarantiBank" bölümünden okunur.
/// </summary>
public class GarantiBankOptions
{
    public const string SectionName = "GarantiBank";

    /// <summary>TEST veya PROD</summary>
    public string Mode { get; set; } = "TEST";

    /// <summary>Garanti Bankası 3D Secure gateway URL'i</summary>
    public string Url3DGate { get; set; } = string.Empty;

    /// <summary>Provision şifresi (terminal şifresi)</summary>
    public string ProvisionPassword { get; set; } = string.Empty;

    /// <summary>3D Store Key</summary>
    public string StoreKey { get; set; } = string.Empty;

    /// <summary>Merchant ID (İşyeri numarası)</summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>Terminal ID</summary>
    public string TerminalId { get; set; } = string.Empty;

    /// <summary>Terminal kullanıcı ID (PROVAUT gibi)</summary>
    public string TerminalUserId { get; set; } = string.Empty;

    /// <summary>API versiyonu (örn: v0.01)</summary>
    public string ApiVersion { get; set; } = "v0.01";

    /// <summary>Başarısız ödeme sonrası yönlendirilecek URL. :paymentId ile değiştirilir.</summary>
    public string FailUrl { get; set; } = string.Empty;

    /// <summary>Başarılı ödeme sonrası yönlendirilecek URL. :paymentId ile değiştirilir.</summary>
    public string SuccessUrl { get; set; } = string.Empty;

    /// <summary>Güvenlik seviyesi (örn: 3D_PAY)</summary>
    public string SecurityLevel { get; set; } = "3D_PAY";

    /// <summary>
    /// true ise geliştirici modunda hash doğrulaması atlanır.
    /// Production'da kesinlikle false olmalıdır.
    /// </summary>
    public bool SkipHashVerification { get; set; } = false;
}


