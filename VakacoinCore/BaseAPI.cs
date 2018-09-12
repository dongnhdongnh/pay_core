using System;

namespace VakacoinCore
{
    public class BaseAPI
    {
        protected Uri HOST = new Uri("http://127.0.0.1:8000");

        public BaseAPI(){}

        public BaseAPI(string host)
        {
            HOST = new Uri(host);
        }
    }
}
