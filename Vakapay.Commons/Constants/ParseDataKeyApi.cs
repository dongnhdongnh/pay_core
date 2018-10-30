namespace Vakapay.Commons.Constants
{
    public class ParseDataKeyApi
    {
        //Pass from filter to Controller
        public const string KEY_PASS_DATA_USER_MODEL = "UserModel";
        public const string KEY_PASS_DATA_GET_OFFSET = "offset";
        public const string KEY_PASS_DATA_GET_LIMIT = "limit";
        public const string KEY_PASS_DATA_GET_FILTER = "filter";
        public const string KEY_PASS_DATA_GET_SORT = "sort";
        public const string KEY_PASS_DATA_GET_CODE = "code";
        
        
        /*-------------------------------------------------------------------User Controller----------------------------------------------------*/
        //Get data user claim from vakaId
        public const string KEY_CLAIM_GET_DATA_USER_IDENTITY = "userInfo";
        
        //User controller UpdateUserProfile
        public const string KEY_USER_UPDATE_PROFILE_ADDRESS_1 = "streetAddress1";
        public const string KEY_USER_UPDATE_PROFILE_ADDRESS_2 = "streetAddress2";
        public const string KEY_USER_UPDATE_PROFILE_CITY = "city";
        public const string KEY_USER_UPDATE_PROFILE_POSTAL_CODE = "postalCode";
        
        //User controller UpdatePreferences
        public const string KEY_USER_UPDATE_PREFERENCES_CURRENCY = "currencyKey";
        public const string KEY_USER_UPDATE_PREFERENCES_TIMEZONE = "timezoneKey";
        
        //User controller Update Notification
        public const string KEY_USER_UPDATE_NOTIFICATION = "notifications";
        //User controller Update Avatar
        public const string KEY_USER_UPDATE_AVATAR = "image";
        
        
        
        /*-------------------------------------------------------------------TwoFA Controller----------------------------------------------------*/
        
        //TwoFa controller VerifyCodeTransaction
        public const string KEY_TWO_FA_VERIFY_CODE_TRANSACTION_SMS = "SMScode";
        
        //TwoFa controller SendCode
        public const string KEY_TWO_FA_SEND_CODE_ACTION = "action";
        
        //TwoFa controller UpdateOption
        public const string KEY_TWO_FA_UPDATE_OPTION = "option";
        public const string KEY_TWO_FA_UPDATE_OPTION_CODE = "code";
        
        //TwoFa controller VerifyCodeEnableGoogle
        public const string KEY_TWO_FA_VERIFY_CODE_ENABLE_GOOGLE_TOKEN = "token";
        
        //TwoFa controller VerifyCodeEnable
        public const string KEY_TWO_FA_VERIFY_CODE_ENABLE_CODE = "code";
        
        //TwoFa controller VerifyCodeDisable
        public const string KEY_TWO_FA_VERIFY_CODE_DISABLE_CODE = "code";
        
       
        
        /*-------------------------------------------------------------------Security Controller----------------------------------------------------*/
        
        //Security controller UpdateCloseAccount
        public const string KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_CODE = "code";
        public const string KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_STATUS = "status";
        public const string KEY_SECURITY_UPDATE_CLOSE_ACCOUNT_PASSWORD = "password";
        
        //Security controller VerifyPassword
        public const string KEY_SECURITY_VERIFY_PASSWORD = "password";
        
        /*-------------------------------------------------------------------ApiAccess Controller----------------------------------------------------*/
        
        //ApiAccess controller data update and add
        public const string KEY_API_ACCESS_DATA = "data";
        public const string KEY_API_ACCESS_DATA_NOTIFY = "notificationUrl";
        public const string KEY_API_ACCESS_DATA_IP = "allowedIp";
        public const string KEY_API_ACCESS_DATA_APIS = "apis";
        public const string KEY_API_ACCESS_DATA_WALLETS = "wallets";
       
    }
}