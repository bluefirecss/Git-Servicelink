using System;

namespace ServicelinkASAPMobile
{
	public static class ProjConfiguration
	{
		private const string wsUrl = "https://staging.wise.servicelinkasap.com/fieldservices/fieldservice.asmx/";
		public static string deviceType = "Android";
		public static string appVersion = "1.0.0";
	
		public enum WebMethod
		{
			AuthenticateDevice = 1,
			GetPostings = 2,
			GetSales = 3
		}

		public static string GetWebMehtod(WebMethod method){

			return wsUrl + method.ToString ();
		}
	}
}

