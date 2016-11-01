using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SecurAX.Logger
{
    // This was class was taken from an ASP.NET forum article on Logging.
    // http://www.asp.net/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/aspnet-error-handling
    // This is a very basic logging utility and future work will involve adding a more mature library for LOGGING.

    // Create our own utility for exceptions
    public sealed class Logger
    {
        // All methods are static, so this can be private
        private Logger()
        { }

        // Log an Exception
        public static void LogException(Exception exc, string source, string function)
        {

            string employee_id = string.Empty;

            if (HttpContext.Current.Session["employee_id"] != null)
            {
                employee_id = HttpContext.Current.Session["employee_id"].ToString();
                if (employee_id == "")
                    employee_id = "admin";
            }
            string date = DateTime.Now.ToString("yyyyMMdd");
            
            // Include logic for logging exceptions
            // Get the absolute path to the log file
            string logFile = "~/logs/errors-" + employee_id + "-" + date + ".txt";
            logFile = HttpContext.Current.Server.MapPath(logFile);

            //if (!File.Exists(logFile))
                //File.Create(logFile);

            // Open the log file for append and write the log
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine("********** {0} **********", DateTime.Now);
            if (exc.InnerException != null)
            {
                sw.Write("Inner Exception Type: ");
                sw.WriteLine(exc.InnerException.GetType().ToString());
                sw.Write("Inner Exception: ");
                sw.WriteLine(exc.InnerException.Message);
                sw.Write("Inner Source: ");
                sw.WriteLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    sw.WriteLine("Inner Stack Trace: ");
                    sw.WriteLine(exc.InnerException.StackTrace);
                }
            }
            sw.Write("Exception Type: ");
            sw.WriteLine(exc.GetType().ToString());
            sw.WriteLine("Exception: " + exc.Message);
            sw.WriteLine("Source: " + source);
            sw.WriteLine("Source: " + function);
            sw.WriteLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                sw.WriteLine(exc.StackTrace);
                sw.WriteLine();
            }
            sw.Close();
        }

        
    }
}