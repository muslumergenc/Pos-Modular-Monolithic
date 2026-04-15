namespace Pos.Providers.GarantiBank;
public static class Constants
{
    public const string PaymentTemplate = """
                                          <form id="frm-payment-go-to-bank" style="display: none !important" method="post" role="form" action="{gateway}">
                                             <input type="hidden" name="mode" id="mode" value="{mode}"/>
                                             <input type="hidden" name="apiversion" id="apiversion" value="{apiversion}"/>
                                             <input type="hidden" name="secure3dsecuritylevel" id="secure3dsecuritylevel" value="{secure3dsecuritylevel}"/>
                                             <input type="hidden" name="terminalprovuserid" id="terminalprovuserid" value="PROVAUT"/>
                                             <input type="hidden" name="terminalmerchantid" id="terminalmerchantid" value="{terminalmerchantid}"/>
                                             <input type="hidden" name="terminaluserid" id="terminaluserid" value="{terminaluserid}"/>
                                             <input type="hidden" name="terminalid" id="terminalid" value="{terminalid}"/>
                                             <input type="hidden" name="orderid" id="orderid" value="{orderid}"/>
                                             <input type="hidden" name="errorurl" id="errorurl" value="{errorurl}"/>
                                             <input type="hidden" name="successurl" id="successurl" value="{successurl}"/>
                                             <input type="hidden" name="customeremailaddress" id="customeremailaddress" value="{customeremailaddress}" />
                                             <input type="hidden" name="customeripaddress" id="customeripaddress" value="{customeripaddress}" />
                                             <input type="hidden" name="companyname" id="companyname" Value="ZARENGRUP"/>
                                             <input type="hidden" name="lang" id="lang" value="{language}"/>
                                             <input type="hidden" name="txnamount" id="txnamount" value="{txnamount}"/>
                                             <input type="hidden" name="txninstallmentcount" id="txninstallmentcount" value="{txninstallmentcount}"/>
                                             <input type="hidden" name="txncurrencycode" id="txncurrencycode" value="{txncurrencycode}"/>
                                             <input type="hidden" name="txntimestamp" id="txntimestamp" value="{txntimestamp}"/>
                                             <input type="hidden" name="secure3dhash" id="secure3dhash" value="{secure3dhash}"/>
                                             <input type="hidden" name="cardholdername" value="{cardholdername}">
                                             <input type="hidden" name="cardnumber" value="{cardnumber}" />
                                             <input type="hidden" name="cardexpiredateyear" value="{cardexpiredateyear}">
                                             <input type="hidden" name="cardexpiredatemonth" value="{cardexpiredatemonth}">
                                             <input type="hidden" name="cardcvv2" value="{cardcvv2}">
                                             <input type="hidden" name="txntype" id="txntype" value="sales"/>
                                             <input type="hidden" name="refreshtime" id="refreshtime" value="1"/>
                                           </form>
                                          """;
    public const string MdStatus3DSecureSignature = "0";
    public const string MdStatusSuccess = "1";
    public const string MdStatusCardNotSuitable = "2";
    public const string MdStatusCardProviderNotSupported = "3";
    public const string MdStatusVerificationAttempt = "4";
    public const string MdStatusCanNotVerify = "5";
    public const string MdStatus3DSecure = "6";
    public const string MdStatusSystemError = "7";
    public const string MdStatusInvalidCardNumber = "8";
    public const string MdStatusMerchantNotRegistered = "9";
}