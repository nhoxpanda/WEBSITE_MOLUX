using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

/// <summary>
/// Summary description for SendEmail
/// Article by vithal wadje http://www.c-sharpcorner.com/Authors/0c1bb2/
/// Facebook Profile:   www.facebook.com/vithal.wadje
//twitter Profile      :https://twitter.com/vithalwadjeC97
//LinedIn Profile    : http://www.linkedin.com/pub/vithal-wadje/69/83a/330

/// </summary>
public static class SendEmail
{
    public static string Pass, FromEmailid, HostAdd;

    public static void Email_With_CCandBCC(string FromEmailid, string Pass, String ToEmail, String Subj, string Message)
    {
        //Reading sender Email credential from web.config file
        HostAdd = "smtp.gmail.com";

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