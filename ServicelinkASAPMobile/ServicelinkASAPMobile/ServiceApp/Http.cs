using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Json;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.ViewModels;

namespace ServicelinkASAPMobile
{
    public class Http
    {
		public static readonly Http Shared = new Http ();
        private static string ws_url = "https://staging.wise.servicelinkasap.com/fieldservices/fieldservice.asmx/";
		public AuthenticationHeader userAuth { get; set; }
		public Posting CurrentAssignment { get; set; }

        WebServiceClient client = new WebServiceClient(ws_url);

		public Http(){
			CurrentAssignment = new Posting ();
		}

        static HttpWebRequest CreateRequest(string webMethod)
        {
            var request = (HttpWebRequest)WebRequest.Create(ws_url + webMethod);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            return request;
        }

        protected static string ReadResponseText(HttpWebRequest req)
        {
            using (WebResponse resp = req.GetResponse())
            {
                using (Stream s = (resp).GetResponseStream())
                {
                    using (var r = new StreamReader(s, Encoding.UTF8))
                    {
                        return r.ReadToEnd();
                    }
                }
            }
        }

        public async Task<string> Login(string username, string password, string version, string deviceType)
        {
            AccountResponse response;
            AuthenticationHeader authHeader = new AuthenticationHeader();
            authHeader.Username = username;
            authHeader.Password = password;
            authHeader.Version = version;
            authHeader.DeviceType = deviceType;
            string error = "";
            try
            {
                var request = Task.Run(() => response = client.UserAuthenticate(authHeader));
                response = await request;
                if (response.Success)
                {

                    //TO DO: save user last login time
                    return error;
                }
                else
                {
                    error = String.Format("Login Failed: {0}", response.Error);
                }
            }
            catch (Exception ex)
            {
                error = String.Format("Login failed for uncaught exceptions: {0}", ex.Message);
            }
            return error;
        }

        List<Posting> assignments;
        public async Task<List<Posting>> GetPostings(AuthenticationHeader authHeader)
        {
            assignments = new List<Posting>();
            assignments = await Task.Factory.StartNew(() =>
            {
                try
                {
                    var content = Encoding.UTF8.GetBytes(userAuth.GetJson(authHeader));

                    var request = CreateRequest("GetPostings");
                    request.Method = "POST";
                    request.ContentLength = content.Length;

                    using (Stream s = request.GetRequestStream())
                    {
                        s.Write(content, 0, content.Length);
                    }
                    string response = ReadResponseText(request);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Posting>>(response);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return new List<Posting>();

                }
            });

            return assignments;
        }

        public async Task<SyncResult> UploadAssignment(AuthenticationHeader authHeader, Posting[] assignments)
        {
            SyncResult syncresult;
            syncresult = await Task.Factory.StartNew(() =>
            {
                try
                {
                    //string assignmentsJson = Newtonsoft.Json.JsonConvert.SerializeObject(assignments);
                    //string authHeaderJson = Newtonsoft.Json.JsonConvert.SerializeObject(authHeader);
                    Dictionary<string, object> parameters = new Dictionary<string, object> { { "updatedPostings", assignments } ,
                                                                                             { "authHeader", authHeader } 
                                                                                            };                
                    string paraJson = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);

                    var content = Encoding.UTF8.GetBytes(paraJson);

                    var request = CreateRequest("UpdatePostings");
                    request.Method = "POST";
                    request.ContentLength = content.Length;

                    using (Stream s = request.GetRequestStream())
                    {
                        s.Write(content, 0, content.Length);
                    }
                    string response = ReadResponseText(request);
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncResult>(response);
                    if (result.Success)
                    { syncresult = result; }
                    return result;
                }
                catch (Exception ex)
                {
                    return new SyncResult
                    {
                        Success = false,
                        Message = ex.Message,
                    };
                }
            });
            return syncresult;
        }

        public async Task<SyncResult> UploadDocuments(AuthenticationHeader authHeader, Document[] docs)
        {
            SyncResult syncresult;
            syncresult = await Task.Factory.StartNew(() =>
            {
                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object> { { "documents", docs } ,
                                                                                             { "authHeader", authHeader } 
                                                                                            };
                    string paraJson = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);

                    var content = Encoding.UTF8.GetBytes(paraJson);

                    var request = CreateRequest("UpdateDocuments");
                    request.Method = "POST";
                    request.ContentLength = content.Length;

                    using (Stream s = request.GetRequestStream())
                    {
                        s.Write(content, 0, content.Length);
                    }
                    string response = ReadResponseText(request);
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncResult>(response);
                    if (result.Success)
                    { syncresult = result; }
                    return result;
                }
                catch (Exception ex)
                {
                    return new SyncResult
                    {
                        Success = false,
                        Message = ex.Message,
                    };
                }
            });
            return syncresult;
        }

    }
}
