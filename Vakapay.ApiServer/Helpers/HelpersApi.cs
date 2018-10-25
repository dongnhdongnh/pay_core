using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
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
                    case ActionLog.UPDATE_OPTION_VETIFY:
                        newSecret.UpdateOptionVerification = secret;
                        break;
                    case ActionLog.API_ACCESS_ADD:
                        newSecret.ApiAccessAdd = secret;
                        break;
                    case ActionLog.API_ACCESS_EDIT:
                        newSecret.ApiAccessEdit = secret;
                        break;
                    case ActionLog.API_ACCESS:
                        newSecret.ApiAccess = secret;
                        break;
                    case ActionLog.SEND_TRSANSACTION:
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