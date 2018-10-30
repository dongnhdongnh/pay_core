namespace Vakapay.ApiServer.Models
{
    public class KeyLogger
    {
        public const string DEVICE_DELETE = "DEVICE_DELETE : ";
        public const string LOG_DELETE = "LOG_DELETE : ";
        public const string LOG_LIST = "LOG_LIST : ";
        public const string DEVICE_LIST = "DEVICE_LIST : ";


        public const string API_ACCESS_GET_INFO = "API_ACCESS_GET_INFO : ";
        public const string API_ACCESS_GET_LIST = "API_ACCESS_GET_LIST : ";
        public const string API_ACCESS_EDIT = "API_ACCESS_EDIT : ";
        public const string API_ACCESS_ADD = "API_ACCESS_ADD : ";
        public const string API_ACCESS_DELETE = "API_ACCESS_DELETE : ";
        public const string API_ACCESS_DISABLE = "API_ACCESS_DISABLE : ";
        public const string API_ACCESS_ENABLE = "API_ACCESS_ENABLE : ";
        public const string API_ACCESS_DETAIL = "API_ACCESS_DETAIL : ";
        public const string API_ACCESS_VERIFY = "API_ACCESS_VERIFY : ";


        public const string SECURITY_GET_INFO = "SECURITY_GET_INFO : ";
        public const string SECURITY_LOCK_SCREEN_UPDATE = "SECURITY_LOCK_SCREEN_UPDATE : ";
        public const string SECURITY_LOCK_SCREEN_UNLOCK = "SECURITY_LOCK_SCREEN_UNLOCK : ";

        public const string TWOFA_OPTION_UPDATE = "TWOFA_OPTION_UPDATE : ";
        public const string TWOFA_ENABLE_UPDATE = "TWOFA_ENABLE_UPDATE : ";
        public const string TWOFA_ENABLE_VERIFY = "TWOFA_ENABLE_VERIFY : ";
        public const string TWOFA_DISABLE_VERIFY = "TWOFA_DISABLE_VERIFY : ";
        public const string TWOFA_SEND_TRANSACTION_VERIFY = "TWOFA_SEND_TRANSACTION_VERIFY : ";
        public const string TWOFA_REQUIRED_SEND_CODE = "TWOFA_REQUIRED_SEND_CODE : ";

        public const string USER_AVATAR = "USER_AVATAR : ";
        public const string USER_UPDATE_PREFERENCES = "USER_UPDATE_PREFERENCES : ";
        public const string USER_UPDATE_NOTIFICATION = "USER_UPDATE_NOTIFICATION : ";
        public const string USER_UPDATE = "USER_UPDATE : ";
    }
}