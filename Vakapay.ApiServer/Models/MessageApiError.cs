namespace Vakapay.ApiServer.Models
{
    public static class MessageApiError
    {
        public const string PARAM_INVALID = "Param is invalid";
        public const string DATA_NOT_FOUND = "Data not found";
        public const string USER_NOT_EXIT = "User not exit";
        public const string SMS_ERROR = "Can't send code sms";
        public const string SMS_VERIFY_ERROR = "Verify fail";
    }
}