using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.EntityClient;
using System.Data.SqlClient;
using LoginForm.Models;
using System.Web.Routing;
using System.IO;
using System.Data;
using System.Net;
using System.Xml;
using System.Net.Mail;

namespace LoginForm.Controllers
{
    public class LoginController : Controller
    {
        //
      static int count = 0;
        // GET: /Login/
        Database1Entities cx = new Database1Entities();
       
     
        public ActionResult login()
        {
            List<Login> list = cx.Logins.ToList();
            
            return View(list);
        }

        public ActionResult verify()
        {
            List<Login> list = cx.Logins.ToList();
            String userName = Request["us"];
            String pass = Request["pass"];
           

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UserName == userName && list[i].Password == pass)
                {
                    int id = list[i].Id;
                    if (pass.Length < 4)
                    {
                        //email(id);
                        return View("ValidatePassword" , id);

                    }
                    
                    return View("SecQuestion", id);    
                }
            }
            return RedirectToAction("Invalid");
        }

        public ActionResult ValidatePassword(String id)
        {
            return View();
        }

        public ActionResult SecQuestion(String id)
        {
            return View(int.Parse(id));
        }

      public ActionResult validate(int id)
        {
            List<Login> list = cx.Logins.ToList();
            String ans = Request["ans"];
            Login log = cx.Logins.Find(id);

            if (count > 3)
                    count = 0;
            if (log.Answer == ans)
            {
                return RedirectToAction("successfull");
            }
            if (count < 3)
            {
                count++;
                return View("InvalidSecQ" , log.Id);
            }
            else {

                return View("login");
            }
           
        }


      public ActionResult Invalid()
      {
          return View();
      }

        
        public ActionResult successfull()
        {
            return View();
        }


        [HttpPost]
        public ViewResult email(int id)
        {
            Login log = cx.Logins.Find(id);
            String email = log.Email;
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("acmikrosoft64@gmail.com"); //acmikrosoft64
                mail.Subject = "Password Updation";
                string Body = "Kindly Update Your Password with more then 4 characters";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("acmikrosoft64@gmail.com", "zaryab123");// Enter seders User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return View("ValidatePassword" , log);
        }

        public ActionResult InvalidSecQ(int id)
        {
            return View(id);
        }
        public static string GetIPAddress(HttpRequestBase request)
        {
            string ip;
            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch { ip = null; }

            return ip;
        }

        private DataTable GetLocation(string ipaddress)
        {
            WebRequest rssReq = WebRequest.Create("http://freegeoip.appspot.com/xml/" + ipaddress);
            WebProxy px = new WebProxy("http://freegeoip.appspot.com/xml/" + ipaddress, true);
            rssReq.Proxy = px;
            rssReq.Timeout = 2000;
            try
            {
                WebResponse rep = rssReq.GetResponse();
                XmlTextReader xtr = new XmlTextReader(rep.GetResponseStream());
                DataSet ds = new DataSet();
                ds.ReadXml(xtr);
                return ds.Tables[0];
            }
            catch
            {
                return null;
            }
        }
    }
}
