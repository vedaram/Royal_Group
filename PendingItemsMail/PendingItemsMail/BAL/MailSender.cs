using System;
using System.Collections.Generic;
using System.Text;
//using System.Web.Mail;
using System.Net.Mail;
using System.IO;




namespace PendingItemsMail
{
    class MailSender
    {

        public static bool SendEmail(string managerHRemail, string pSubject, string pBody, string pcc, string managername, string strerrorpath)
        {
            string server, port, userid, password, userto;
            string ccmail = string.Empty;
            string managerhremail = string.Empty;
            bool sendflag = false;
            DBCom.GetMailCredentials(out server, out port, out userid, out userto, out password, out ccmail);

            MailAddress from = new MailAddress(userid);
            MailAddress to = new MailAddress(managerHRemail);
            //MailAddress copy = new MailAddress
            MailMessage myMail = new MailMessage(from, to);
            myMail.Priority = MailPriority.High;
            myMail.Subject = pSubject;
            myMail.Body = pBody;

            // myMail.CC.Add("");

            myMail.IsBodyHtml = true;
            myMail.Body = pBody;

            //================================================
            myMail.Subject = "Pending Manual Punch, OverTime and OUT OF OFFICE approval record as on  " + System.DateTime.Now.ToString("dd-MMM-yyyy");
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;
            //myMail.Body = "Dear " + managername + ",<br/>" + "<br/>" + "<br/>" + "<br/>";
            //myMail.Body = myMail.Body + "Please find the attached daily Report of last week for your reportee employee. " + "<br/>" + "<br/>";
            ////myMail.Body = myMail.Body + "Candidate Name : <b>" + strCandidateName + "</b><br/>";
            ////myMail.Body = myMail.Body + "Case Reference No. : <b>" + strCompanyRefNo + "</b><br/>" + "<br/>" + "<br/>" + "<br/>";
            ////mail.Body = mail.Body + "Thanks & Regards," + "<br/>";
            ////mail.Body = mail.Body + strTeamMemberName + "<br/>";

            //// Details for Signature
            //myMail.Body = myMail.Body + "<br /><br /> Thanks and Regards,<br /> " + from + "<br /> <b>";
            ////myMail.Body = myMail.Body + "<p ><b>http://www.securax.in</b><br/><b>Mobile: 95444444 | Direct Tel: + 91[124] 4423 807 | Fax: +91 [124] 4423 815</b><br/>";
            ////myMail.Body = myMail.Body + "E-mail id: " + "" + " </p><br/><br/><br/><br/>";
            //myMail.Body = myMail.Body + "Please do not reply on this mail, because it is system generated mail!";


            //===============================================
            SmtpClient client = new SmtpClient(server);
            server = server.ToLower();
            if (server == "smtp.gmail.com")
            {
                client.EnableSsl = true;
            }
            else
            {
                client.EnableSsl = false;
            }
            client.Credentials = new System.Net.NetworkCredential(userid, password);
            client.Port = Convert.ToInt32(port);
            // client.EnableSsl = true;
            //if (pAttachmentPath != null)
            //    myMail.Attachments.Add(new Attachment(pAttachmentPath));
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;

            try
            {
                client.Send(myMail);
                myMail.Dispose();
                sendflag = true;
            }
            catch (Exception ex)
            {

                ErrorLog.ErrorLogfile(@"" + strerrorpath + "", ex.Message, ex.StackTrace);
                sendflag = false;

            }
            return sendflag;

        }
        public static void DeleteUploadedFile(string strFilePath, string strerrorpath)
        {
            string directoryName = Path.GetDirectoryName(strFilePath);

            string[] arrfile = Directory.GetFiles(directoryName);
            foreach (string afile in arrfile)
            {
                try
                {
                    File.Delete(afile);
                }
                catch (Exception ex)
                {

                    ErrorLog.ErrorLogfile(@"" + strerrorpath + "", ex.Message, ex.StackTrace);
                }
            }


        }

    }
}
