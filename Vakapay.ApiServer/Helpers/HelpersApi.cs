using System;
using Microsoft.AspNetCore.Http;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
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

        public static string SendCodeSms(string secret)
        {
            var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
            var code = authenticator.GetCode(secret);
            Console.WriteLine(code);
            return code;
        }

        public static string CheckToken(User userModel, string action)
        {
            try
            {
                var newSecret = new ActionCode();

                if (string.IsNullOrEmpty(userModel.SecretAuthToken))
                {
                    switch (action)
                    {
                        case ActionLog.TWOFA_ENABLE:
                            newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.SEND_TRSANSACTION:
                            newSecret.SendTransaction = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.TWOFA_DISABLE:
                            newSecret.TwofaDisable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.UPDATE_NOTIFICATION:
                            newSecret.UpdateOptionVerification = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.LOCK_SCREEN:
                            newSecret.LockScreen = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                    }
                }
                else
                {
                    newSecret = ActionCode.FromJson(userModel.SecretAuthToken);

                    switch (action)
                    {
                        case ActionLog.TWOFA_ENABLE:
                            if (string.IsNullOrEmpty(newSecret.TwofaEnable))
                            {
                                newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.SEND_TRSANSACTION:
                            if (string.IsNullOrEmpty(newSecret.SendTransaction))
                            {
                                newSecret.SendTransaction = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.TWOFA_DISABLE:
                            if (string.IsNullOrEmpty(newSecret.TwofaDisable))
                            {
                                newSecret.TwofaDisable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.UPDATE_NOTIFICATION:
                            if (string.IsNullOrEmpty(newSecret.UpdateOptionVerification))
                            {
                                newSecret.UpdateOptionVerification = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.LOCK_SCREEN:
                            if (string.IsNullOrEmpty(newSecret.LockScreen))
                            {
                                newSecret.LockScreen = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                    }
                }

                return ActionCode.ToJson(newSecret);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}