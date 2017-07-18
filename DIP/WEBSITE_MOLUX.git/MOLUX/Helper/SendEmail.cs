using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

public static class SendEmail
{
    public static string Pass, FromEmailid, HostAdd;

    public static void Email_With_CCandBCC(string host, string FromEmailid, string Pass, String ToEmail, String Subj, string Message)
    {
        //Reading sender Email credential from web.config file
        HostAdd = host;

        //creating the object of MailMessage
        MailMessage mailMessage = new MailMessage();

        mailMessage.From = new MailAddress(FromEmailid); //From Email Id
        mailMessage.Subject = Subj; //Subject of Email
        mailMessage.Body = Message; //body or message of Email
        mailMessage.IsBodyHtml = true;

        string[] ToMuliId = ToEmail.Split(',');
        foreach (string ToEMailId in ToMuliId)
        {
            mailMessage.To.Add(new MailAddress(ToEMailId)); //adding multiple TO Email Id
        }

        SmtpClient smtp = new SmtpClient(); // creating object of smptpclient
        smtp.Host = HostAdd; //host of emailaddress for example smtp.gmail.com etc

        smtp.UseDefaultCredentials = true;
        smtp.Credentials = new System.Net.NetworkCredential(FromEmailid, Pass);
        //network and security related credentials

        smtp.EnableSsl = true;
        NetworkCredential NetworkCred = new NetworkCredential();
        NetworkCred.UserName = mailMessage.From.Address;
        NetworkCred.Password = Pass;
        //smtp.UseDefaultCredentials = true;
        smtp.Port = 587;
        smtp.Send(mailMessage); //sending Email
    }


}