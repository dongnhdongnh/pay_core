namespace Vakapay.ApiServer.Models
{
    public static class MessageApiError
    {
        public const string ParamInvalid = "Param is invalid";
        public const string DataNotFound = "Data not found";
        public const string UserNotFound = "User is not found";
        public const string SmsError = "Can't send code sms";
        public const string SmsVerifyError = "Verify fail";
    }
}