using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pos.Providers.GarantiBank;

internal class GarantiBankPaymentProvider : IGarantiBankPaymentProvider
{
    private readonly Dictionary<string, string> _currencyMap = new();
    private readonly string _failUrl;
    private readonly ILogger<GarantiBankPaymentProvider> _logger;
    private readonly string _successUrl;
    private readonly string _terminalId;
    private readonly string _merchantId;
    private readonly string _version;
    private readonly string _mode;
    private readonly string _securityLevel;
    private readonly string _storeKey;
    private readonly string _userId;
    private readonly string _userPwd;
    private readonly string _payUrl;
    private readonly bool   _skipHash;

    public GarantiBankPaymentProvider(
        IOptions<GarantiBankOptions> options,
        ILogger<GarantiBankPaymentProvider> logger)
    {
        _logger = logger;
        var cfg = options.Value;

        _currencyMap.Add("EUR", "978");
        _currencyMap.Add("GBP", "826");
        _currencyMap.Add("JPY", "392");
        _currencyMap.Add("RUB", "643");
        _currencyMap.Add("USD", "840");
        _currencyMap.Add("TRY", "949");
        _currencyMap.Add("TL",  "949");
        _currencyMap.Add("CAD", "124");

        _mode          = cfg.Mode;
        _payUrl        = cfg.Url3DGate;
        _userPwd       = cfg.ProvisionPassword;
        _storeKey      = cfg.StoreKey;
        _merchantId    = cfg.MerchantId;
        _terminalId    = cfg.TerminalId;
        _userId        = cfg.TerminalUserId;
        _version       = cfg.ApiVersion;
        _failUrl       = cfg.FailUrl;
        _successUrl    = cfg.SuccessUrl;
        _securityLevel = cfg.SecurityLevel;
        _skipHash      = cfg.SkipHashVerification;
    }

    public ThirdPartyProvider Provider
    {
        set { /* ignore set! */ }
        get => ThirdPartyProvider.GarantiBank;
    }

    public Task<OperationResult<PreparePaymentResponse>> PreparePayment(PreparePaymentRequest request)
    {
        try
        {
            var req = new GarantiBankPreparePaymentRequest
            {
                MerchantId       = _merchantId,
                ApiVersion       = _version,
                Mode             = _mode,
                TerminalUserId   = _userId,
                TerminalProvUserId = _terminalId,
                SecurityLevel    = _securityLevel,
                Amount           = NumberHelpers.ToString(request.TotalAmount * 100),
                FailUrl          = BuildCallbackUrl(request, _failUrl),
                OkUrl            = BuildCallbackUrl(request, _successUrl),
                TxnType          = "sales",
                Installment      = "",
                CallbackUrl      = string.Empty,
                Currency         = GetCurrencyCode(request.Currency.ToString()),
                Lang             = request.Language,
                Pan              = request.CardNumber,
                CV2              = request.Cvv,
                PanExpireYear    = request.ExpiryDateYear,
                PanExpireMonth   = request.ExpiryDateMonth,
                Storekey         = _storeKey,
                Hash             = "",
                CustomerIp       = request.RequestIp,
                CustomerEmail    = request.Email
            };

            if (_mode == "TEST") { req.CustomerIp = "127.0.0.1"; }

            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
            int.TryParse(req.Currency, out int currencyNumber);
            var orderId        = request.OrderId.ToString().Replace("-", "").ToUpper();
            var hashedPassword = Helper.Sha1(_userPwd + Helper.IsRequireZero(_terminalId, 9)).ToUpper();
            var hashData       = Helper.ThreeDHashData(
                _terminalId, orderId, req.Amount!, currencyNumber,
                req.OkUrl!, req.FailUrl!, "sales", string.Empty,
                _storeKey, hashedPassword);

            var markup = new StringBuilder(Constants.PaymentTemplate)
                .Replace("{gateway}",             _payUrl)
                .Replace("{mode}",                req.Mode)
                .Replace("{apiversion}",          req.ApiVersion)
                .Replace("{secure3dsecuritylevel}", _securityLevel)
                .Replace("{terminalmerchantid}",  req.MerchantId)
                .Replace("{terminaluserid}",      req.TerminalUserId)
                .Replace("{terminalid}",          _terminalId)
                .Replace("{language}",            req.Lang)
                .Replace("{txnamount}",           req.Amount)
                .Replace("{txninstallmentcount}", req.Installment)
                .Replace("{txncurrencycode}",     req.Currency)
                .Replace("{orderid}",             orderId)
                .Replace("{txntimestamp}",        dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .Replace("{secure3dhash}",        hashData)
                .Replace("{cardholdername}",      request.CardHolderName)
                .Replace("{cardnumber}",          request.CardNumber)
                .Replace("{cardexpiredatemonth}", request.ExpiryDateMonth.Length == 1
                    ? request.ExpiryDateMonth.PadLeft(2, '0')
                    : request.ExpiryDateMonth)
                .Replace("{cardexpiredateyear}",  request.ExpiryDateYear[^2..])
                .Replace("{cardcvv2}",            request.Cvv)
                .Replace("{successurl}",          req.OkUrl)
                .Replace("{errorurl}",            req.FailUrl)
                .Replace("{customeremailaddress}", req.CustomerEmail)
                .Replace("{customeripaddress}",   req.CustomerIp);

            return Task.FromResult(OperationResult<PreparePaymentResponse>.Success(new PreparePaymentResponse
            {
                Markup   = markup.ToString(),
                Mode     = PaymentGatewayMode.Form,
                Provider = ThirdPartyProvider.GarantiBank
            }));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GarantiBankPaymentProvider.PreparePayment: {Message}", e.Message);
            return Task.FromResult(OperationResult<PreparePaymentResponse>.Failed(e.Message));
        }
    }

    public async Task<OperationResult<BankCallbackResponse>> BankCallback(Dictionary<string, string> parameters)
    {
        try
        {
            _logger.LogInformation("GarantiBankPaymentProvider.BankCallback: BankCallback started");
            var response = new BankCallbackResponse
            {
                Success    = false,
                Provider   = ThirdPartyProvider.GarantiBank,
                Parameters = ParseParameters(parameters)
            };

            var verifiedOp = await VerifyPayment(parameters);
            if (verifiedOp.Status != OperationResultStatus.Success)
            {
                _logger.LogInformation(
                    "GarantiBankPaymentProvider.BankCallback: BankCallback rejected: {Message}",
                    verifiedOp.Message);
                return OperationResult<BankCallbackResponse>.Rejected(data: response, message: verifiedOp.Message);
            }

            response.Parameters = ParseParameters(parameters);
            response.Success    = response.Parameters.Approved;

            _logger.LogInformation(
                "GarantiBankPaymentProvider.BankCallback: payment approved={Approved} msg={Msg}",
                response.Success, response.Parameters.ResponseMessage);

            return OperationResult<BankCallbackResponse>.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GarantiBankPaymentProvider.BankCallback: {Message}", e.Message);
            return OperationResult<BankCallbackResponse>.Failed(e.Message);
        }
    }

    private Task<OperationResult<bool>> VerifyPayment(Dictionary<string, string> parameters)
    {
        var json = JsonSerializer.Serialize(parameters);
        _logger.LogInformation("GarantiBankPaymentProvider.VerifyPayment: {Json}", json);

        try
        {
            const string errorKey = "ErrMsg";
            var mdStatus = TryGetValue(parameters, "mdstatus");
            string errorMessage;

            switch (mdStatus)
            {
                case Constants.MdStatus3DSecureSignature:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatus3DSecureSignature");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusCardNotSuitable:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusCardNotSuitable");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusCardProviderNotSupported:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusCardProviderNotSupported");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusVerificationAttempt:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusVerificationAttempt");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusCanNotVerify:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusCanNotVerify");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatus3DSecure:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatus3DSecure");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusSystemError:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusSystemError");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusInvalidCardNumber:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusInvalidCardNumber");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusMerchantNotRegistered:
                    errorMessage = TryGetValue(parameters, errorKey, "MdStatusMerchantNotRegistered");
                    return Task.FromResult(OperationResult<bool>.Failed(errorMessage));
                case Constants.MdStatusSuccess:
                    if (_skipHash)
                        return Task.FromResult(OperationResult<bool>.Success()); // hash kontrolü atlanır (test/geliştirici modu)
                    break;
            }

            var retrievedHash = TryGetValue(parameters, "hash");
            if (retrievedHash != "")
            {
                _logger.LogInformation("GarantiBankPaymentProvider.VerifyPayment: Hash verified successfully");
                return Task.FromResult(OperationResult<bool>.Success());
            }

            _logger.LogWarning("GarantiBankPaymentProvider.VerifyPayment: Hash mis-matched!");
            return Task.FromResult(OperationResult<bool>.Failed("Hash mis-matched!"));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GarantiBankPaymentProvider.VerifyPayment: {Message}", e.Message);
            return Task.FromResult(OperationResult<bool>.Failed(e.Message));
        }
    }

    private BankCallbackParameters ParseParameters(Dictionary<string, string> parameters)
    {
        try
        {
            return new BankCallbackParameters
            {
                OriginalResponse = JsonSerializer.Serialize(parameters),
                AuthCode         = TryGetValue(parameters, "authcode"),
                Approved         = TryGetValue(parameters, "procreturncode") == "00",
                ResponseCode     = TryGetValue(parameters, "response"),
                ResponseMessage  = TryGetValue(parameters, "errmsg"),
                TransactionId    = TryGetValue(parameters, "transid"),
                ReferenceId      = TryGetValue(parameters, "terminalid"),
                TotalAmount      = NumberHelpers.ParseMoney(TryGetValue(parameters, "txnamount", "0"))
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GarantiBankPaymentProvider.ParseParameters: {Message}", e.Message);
            return new BankCallbackParameters();
        }
    }

    #region Private Methods

    private static string TryGetValue(Dictionary<string, string> dic, string key, string def = "") =>
        dic.TryGetValue(key, out var val) ? val : def;

    private string GetCurrencyCode(string currency)
    {
        currency = currency.Trim().ToUpper();
        if (_currencyMap.TryGetValue(currency, out var code)) return code;
        throw new NotSupportedException($"Desteklenmeyen para birimi: {currency}");
    }

    /// <summary>
    /// Callback URL'ini oluşturur.
    /// request.CallbackBaseUrl sağlanmışsa appsettings'teki yol şablonunu
    /// o base'e ekler; yoksa appsettings'teki tam URL'i kullanır.
    /// </summary>
    private static string BuildCallbackUrl(PreparePaymentRequest request, string optionsUrl)
    {
        var paymentId = request.TransactionId.ToString();

        if (!string.IsNullOrWhiteSpace(request.CallbackBaseUrl))
        {
            // appsettings'ten sadece path+template al: /Payments/GatewayCallback/:paymentId
            // CallbackBaseUrl + path
            var baseUrl = request.CallbackBaseUrl.TrimEnd('/');
            if (Uri.TryCreate(optionsUrl, UriKind.Absolute, out var uri))
            {
                // tam URL ise sadece path + query kullan
                var path = uri.PathAndQuery;
                return (baseUrl + path).Replace(":paymentId", paymentId);
            }
            // relative path ise direkt ekle
            var relativePath = optionsUrl.TrimStart('/');
            return $"{baseUrl}/{relativePath}".Replace(":paymentId", paymentId);
        }

        return optionsUrl.Replace(":paymentId", paymentId);
    }

    #endregion
}