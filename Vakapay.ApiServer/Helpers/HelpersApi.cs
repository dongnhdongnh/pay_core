using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using UAParser;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;

namespace Vakapay.ApiServer.Helpers
{
    public static class HelpersApi
    {
        public static string GetIp(HttpRequest request)
        {
            var ip = request.Headers["X-Forwarded-For"].ToString();

            if (!string.IsNullOrEmpty(ip))
                ip = request.Headers["X-Real-IP"].ToString();

            return ip;
        }

        public static bool ValidateCurrency(string currency)
        {
            return CryptoCurrency.ALL_NETWORK.Contains(currency);
        }

        public static bool CheckCodeGoogle(string secret, string token)
        {
            var google = new GoogleAuthen.TwoFactorAuthenticator();

            var valid = google.ValidateTwoFactorPIN(secret, token);

            return valid;
        }

        public static bool ValidatePass(string pass)
        {
            const string pattern = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,100}$";
            return Regex.IsMatch(pass, pattern);
        }

        public static bool ValidateSecondPass(string pass)
        {
            const string pattern = @"^([^\s]*)$";
            return Regex.IsMatch(pass, pattern);
        }

        public static bool ValidatePermission(string permission)
        {
            var datas = permission.Split(",");
            if (datas.Length > 0)
            {
                foreach (var data in datas)
                {
                    if (!ApiAccess.LIST_API_ACCESS.ContainsKey(data.Trim()))
                        return false;
                }
            }

            return true;
        }

        public static bool ValidateWallet(string wallets)
        {
            var datas = wallets.Split(",");
            if (datas.Length > 0)
            {
                foreach (var data in datas)
                {
                    if (!CryptoCurrency.ALL_NETWORK.Contains(data.Trim()))
                        return false;
                }
            }

            return true;
        }

        public static bool CheckCodeSms(string secret, string token, User model, int time = 30)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator(null, null, time);
            return authenticator.CheckCode(secret, token, model);
        }

        public static bool CheckUrlValid(string source)
        {
            return Uri.TryCreate(source, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }

        public static string SendCodeSms(string secret, int time = 30)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator(null, null, time);
            var code = authenticator.GetCode(secret);
            Console.WriteLine(code);
            Console.WriteLine(secret);
            return code;
        }

        public static string GetBrowser(HttpRequest request)
        {
            var uaString = request.Headers["User-Agent"].FirstOrDefault();
            var uaParser = Parser.GetDefault();

            string browser = uaParser.ParseUserAgent(uaString).ToString();

            if (browser.ToLower().Contains("chrome"))
                return uaParser.ParseOS(uaString) + ", " + "Chrome";

            if (browser.ToLower().Contains("chromium"))
                return uaParser.ParseOS(uaString) + ", " + "Chromium";

            if (browser.ToLower().Contains("firefox"))
                return uaParser.ParseOS(uaString) + ", " + "Firefox";

            return uaParser.Parse(uaString).ToString();
        }

        public static CheckTokenModel CheckToken(User userModel, string action)
        {
            try
            {
                var secret = TwoStepsAuthenticator.Authenticator.GenerateKey();
                var data = new CheckTokenModel
                {
                    Secret = secret,
                    NewSecret = null
                };
                var newSecret = new ActionCode();

                if (!string.IsNullOrEmpty(userModel.SecretAuthToken))
                {
                    newSecret = ActionCode.FromJson(userModel.SecretAuthToken);
                }

                switch (action)
                {
                    case ActionLog.TWOFA_ENABLE:
                        newSecret.TwofaEnable = secret;
                        break;
                    case ActionLog.CUSTOM_TWOFA:
                        newSecret.CustomTwofa = secret;
                        break;
                    case ActionLog.UPDATE_OPTION_VETIFY:
                        newSecret.UpdateOptionVerification = secret;
                        break;
                    case ActionLog.API_ACCESS_ADD:
                        newSecret.ApiAccessAdd = secret;
                        break;
                    case ActionLog.API_ACCESS_EDIT:
                        newSecret.ApiAccessEdit = secret;
                        break;
                    case ActionLog.API_ACCESS_DELETE:
                        newSecret.ApiAccessDelete = secret;
                        break;
                    case ActionLog.API_ACCESS_STATUS:
                        newSecret.ApiAccessStatus = secret;
                        break;
                    case ActionLog.SEND_TRANSACTION:
                        newSecret.SendTransaction = secret;
                        break;
                    case ActionLog.TWOFA_DISABLE:
                        newSecret.TwofaDisable = secret;
                        break;
                    case ActionLog.LOCK_SCREEN:
                        newSecret.LockScreen = secret;
                        break;
                }

                data.NewSecret = ActionCode.ToJson(newSecret);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string CreateDataError(string message)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_ERROR,
                Message = message
            }.ToJson();
        }
    }
}