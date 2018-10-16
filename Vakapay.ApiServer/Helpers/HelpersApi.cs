using System;
using Microsoft.AspNetCore.Http;
using Vakapay.ApiServer.Models;
using Vakapay.Commons.Constants;
using Vakapay.Models.Entities;

namespace Vakapay.ApiServer.Helpers
{
    public class HelpersApi
    {
        public static string getIp(HttpRequest request)
        {
            string ip = request.Headers["X-Forwarded-For"].ToString();

            if (!string.IsNullOrEmpty(ip))
                ip = request.Headers["X-Real-IP"].ToString();

            return ip;
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
                        case ActionLog.TwofaEnable:
                            newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.UpdateOptionVerification:
                            newSecret.UpdateOptionVerification = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                        case ActionLog.LockScreen:
                            newSecret.LockScreen = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            break;
                    }
                }
                else
                {
                    newSecret = ActionCode.FromJson(userModel.SecretAuthToken);

                    switch (action)
                    {
                        case ActionLog.TwofaEnable:
                            if (string.IsNullOrEmpty(newSecret.TwofaEnable))
                            {
                                newSecret.TwofaEnable = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.UpdateOptionVerification:
                            if (string.IsNullOrEmpty(newSecret.UpdateOptionVerification))
                            {
                                newSecret.UpdateOptionVerification = TwoStepsAuthenticator.Authenticator.GenerateKey();
                            }

                            break;
                        case ActionLog.LockScreen:
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
                return null;
            }
        }
    }
}