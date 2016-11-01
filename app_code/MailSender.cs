using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using System.Text;


/// <summary>
/// Summary description for MailSender
/// </summary>
public class MailSender
{
   
    public static bool SendEmail(string pTo, string pTo1, string pSubject, string pBody)
    {
        try
        {
            MailMessage myMail = new MailMessage();
            //myMail.From = new MailAddress("shahul@skylarkmansions.co.in");
            string email = ConfigurationSettings.AppSettings["emailid"].ToString();
            bool ssl = Convert.ToBoolean(ConfigurationSettings.AppSettings["ssl"].ToString());
            myMail.From = new MailAddress(email);
            myMail.To.Add(pTo);
            if (pTo1 != "")
            {
                myMail.CC.Add(pTo1);
            }
            myMail.Subject = pSubject;
            myMail.Body = pBody;
            myMail.Priority = MailPriority.High;
            myMail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.EnableSsl = ssl;
            //client.Timeout = 60000;
            //client.UseDefaultCredentials = false;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            client.Send(myMail);

            return true;
            
        }
        catch (Exception ex)
        {

            ex.Message.ToString();
            return false;
        }
       
    }
       
}

