﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace ServicelinkASAPMobile.ViewModels
{
    public class WebServiceClient
    {
        const int TimeoutSeconds = 30;
        const string user_agent = "XamarinSSO .NET v1.0";
        static Encoding encoding = Encoding.UTF8;
        string auth_api_url;
        string apikey;

        //public WebServiceClient(string apikey) : this("https://auth.xamarin.com", apikey)
        //{
        //}
        //public WebServiceClient(string base_url, string apikey)
        //{
        //    auth_api_url = base_url + "/api/v1/auth";
        //    this.apikey = apikey;
        //}

        public WebServiceClient(string url) {
            auth_api_url = url;
        }

        protected virtual WebRequest SetupRequest(string method, string url)
        {
            WebRequest req = (WebRequest)WebRequest.Create(url);
            req.Method = method;
            if (req is HttpWebRequest)
            {
                ((HttpWebRequest)req).UserAgent = user_agent;
            }
            req.Credentials = new NetworkCredential(apikey, "");
            req.PreAuthenticate = true;
            req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(apikey + ":")));
            req.Timeout = TimeoutSeconds * 1000;
            if (method == "POST" || method == "PUT")
                req.ContentType = "application/x-www-form-urlencoded";

            return req;
        }

        static string GetResponseAsString(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
            {
                return sr.ReadToEnd();
            }
        }

        protected virtual string DoRequest(string endpoint, string method = "GET", string body = null)
        {
            string result = null;
            WebRequest req = SetupRequest(method, endpoint);
            if (body != null)
            {
                byte[] bytes = encoding.GetBytes(body.ToString());
                req.ContentLength = bytes.Length;
                using (Stream st = req.GetRequestStream())
                {
                    st.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (WebResponse resp = (WebResponse)req.GetResponse())
                {
                    result = GetResponseAsString(resp);
                }
            }
            catch (WebException)
            {
                throw;
            }
            return result;
        }

        public AccountResponse UserAuthenticate(AuthenticationHeader authHeader)
        {
            var str = String.Format("authHeader={0}", authHeader);
            string json = DoRequest(auth_api_url, "POST", str);
            return JsonConvert.DeserializeObject<AccountResponse>(json);
        }

        static string UrlEncode(string src)
        {
            if (src == null)
                return null;
            return HttpUtility.UrlEncode(src);
        }
    }
}
