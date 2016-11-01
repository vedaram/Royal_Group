using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PendingItemsMail
{
   public  class ErrorLog
    {
      static string sLogFormat = string.Empty;
      static string sErrorTime = string.Empty;
      
       //string sPathName=string.Empty;
      public static void CreateLogFiles()
        {
            sLogFormat = (DateTime.Now.ToShortDateString().ToString() + " ") + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            sErrorTime = sDay + "-" + sMonth + "-" + sYear;
        }
      public static void ErrorLogfile(string sPathName, string sErrMsg, string sStackTrace)
        {
            CreateLogFiles();
            StreamWriter sw = new StreamWriter(sPathName + "(" + sErrorTime + ").txt", true);
            sw.WriteLine(sLogFormat + sErrMsg);
            sw.WriteLine(sStackTrace);
            sw.WriteLine("___________________________________________________________________________________________________________________");
            sw.Flush();
            sw.Close();
        }
        
        public string Encryptdata(string pass)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[pass.Length];
            encode = Encoding.UTF8.GetBytes(pass);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        public string Decryptdata(string pw)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(pw);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
        public string Emp_Code { get; set; }
        public string Emp_name { get; set; }

    }
}
