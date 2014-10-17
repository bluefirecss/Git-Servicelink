using System;
using Newtonsoft.Json;

namespace ServicelinkASAPMobile
{
    [Serializable()]
	public class User
	{
		public User ()
		{
			UserInfoChanged = false;
			UserLoggedout = false;
		}

		public string UserName { get; set; }

        public string AgentUserID { get; set; }

		public string Password { get; set; }

		public string AppVersion { get; set; }

        public string DeviceType { get; set; }

		public DateTime SyncDateTime { get; set; }

		public DateTime LastSuccessfulLoginDate { get; set; }

		public Boolean UserInfoChanged { get; set; }

		public Boolean UserLoggedout { get; set; }

        public string PushNoticeActive { get; set; }

        public string GetJson(User user)
        {
#if __IOS__
			DeviceType = "iOS";
#elif __ANDROID__
			DeviceType = "Android";
#endif
            UserName = user.UserName;
            Password = user.Password;
            AppVersion = user.AppVersion;
            DeviceType = user.DeviceType;
            AgentUserID = user.AgentUserID;
            SyncDateTime = user.SyncDateTime;
			LastSuccessfulLoginDate = user.LastSuccessfulLoginDate;
			UserInfoChanged = user.UserInfoChanged;
			UserLoggedout = user.UserLoggedout;
            PushNoticeActive = user.PushNoticeActive;

            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
	}
}

