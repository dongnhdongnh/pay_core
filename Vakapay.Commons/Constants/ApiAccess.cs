using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public class ApiAccess
    {
        public static readonly Dictionary<string, string> LIST_API_ACCESS = new Dictionary<string, string>
        {
            {
                "CREATED_ADDRESSES",
                "wallet:addresses:create"
            },
            {
                "READ_ADDRESSES",
                "wallet:addresses:read"
            },
            {
                "READ_DEPOSITS",
                "wallet:deposits:read"
            },
            {
                "CREATED_DEPOSITS",
                "wallet:deposits:create"
            },
            {
                "READ_TRANSACTIONS",
                "wallet:transactions:read"
            },
            {
                "SEND_TRANSACTIONS",
                "wallet:transactions:send"
            },
            {
                "READ_WITHDRAWS",
                "wallet:withdrawals:read"
            },
            {
                "CREATE_WITHDRAWS",
                "wallet:withdrawals:create"
            },
            {
                "USER_MAIL",
                "wallet:user:email"
            },
            {
                "USER_READ",
                "wallet:user:read"
            }
        };
    }
}