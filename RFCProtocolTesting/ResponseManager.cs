using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFCProtocolTesting
{
    public sealed class ResponseManager
    {
        private ResponseManager()
        {
        }

        public ResponseSetting responseSetting = ResponseSetting.Echo;
        public string staticResponse = "";

        public string getResponse(string body)
        {
            switch (responseSetting)
            {
                case ResponseSetting.Echo:
                    return body;
                case ResponseSetting.Static:
                    return staticResponse;
                case ResponseSetting.File:
                    return "File return not implemented";
                default:
                    return body;
            }
        }

        public static ResponseManager Instance { get { return Nested.instance; } }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly ResponseManager instance = new ResponseManager();
        }
    }
}
