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

        public static bool CheckCodeSms(string secret, string token, User model)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            return authenticator.CheckCode(secret, token, model);
        }

        public static bool CheckUrlValid(string source)
        {
            return Uri.TryCreate(source, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }

        public static string SendCodeSms(string secret)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            var code = authenticator.GetCode(secret);
            Console.WriteLine(code);
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

                if (string.IsNullOrEmpty(userModel.SecretAuthToken))
                {
                    switch (action)
                    {
                        case ActionLog.TWOFA_ENABLE:
                            newSecret.TwofaEnable = secret;
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
                        case ActionLog.UPDATE_NOTIFICATION:
                            newSecret.UpdateOptionVerification = secret;
                            break;
                        case ActionLog.LOCK_SCREEN:
                            newSecret.LockScreen = secret;
                            break;
                    }
                }
                else
                {
                    newSecret = ActionCode.FromJson(userModel.SecretAuthToken);

                    switch (action)
                    {
                        case ActionLog.TWOFA_ENABLE:
                            newSecret.TwofaEnable = secret;
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
                        case ActionLog.UPDATE_NOTIFICATION:
                            newSecret.UpdateOptionVerification = secret;
                            break;
                        case ActionLog.LOCK_SCREEN:
                            newSecret.LockScreen = secret;
                            break;
                    }
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