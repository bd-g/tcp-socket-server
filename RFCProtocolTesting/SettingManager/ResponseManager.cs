using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RFCProtocolTesting.Forms;

namespace RFCProtocolTesting.SettingManager
{
    public sealed class ResponseManager
    {
        private ResponseManager()
        {
        }

        public ResponseSetting responseSetting = ResponseSetting.Echo;
        public string staticResponse = "";

        public byte[] getResponse(string body)
        {
            byte[] byteData = new byte[0];
            switch (responseSetting)
            {
                case ResponseSetting.Echo:
                    byteData = Encoding.ASCII.GetBytes(body);
                    return byteData;
                case ResponseSetting.Static:
                    byteData = Encoding.ASCII.GetBytes(staticResponse);
                    return byteData;
                case ResponseSetting.File:
                    if (File.Exists(body))
                    {
                        using (FileStream fs = new FileStream(body, FileMode.Open, FileAccess.Read))
                        {
                            byteData = System.IO.File.ReadAllBytes(body);
                            fs.Read(byteData, 0, System.Convert.ToInt32(fs.Length));
                            fs.Close();
                            return byteData;
                        }
                    }
                    else
                    {
                        byteData = Encoding.ASCII.GetBytes("File not found or invalid path");
                        return byteData;
                    }
                case ResponseSetting.XML:
                    byteData = Encoding.ASCII.GetBytes(body);
                    return byteData;
                default:
                    return byteData;
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
