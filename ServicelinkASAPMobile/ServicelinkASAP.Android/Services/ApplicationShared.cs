using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAP
{
    public class ApplicationShared : Application
    {
        private bool isNetworkActive;
        private bool isWifiActive;
        private bool synced;
        private string dialogTitle;
        private string dialogMsg;
        private DateTime lastActiveDate;
		private Assignment lastassignment;
		private int lastFragmentId;
		private int lastBaseContainerID;
		private string lastSelectedQueueName;
		private MenuItem lastSelectedMenu;
		private bool syncServiceEnabled;
        #region public accessor
		public bool GetSyncServiceEnabled()
		{
			return syncServiceEnabled;
		}
		public void SetSyncServiceEnabled(bool enabled)
		{
			this.syncServiceEnabled = enabled;
		}
        public bool GetNetworkActive()
        {
            return isNetworkActive;
        }
        public void SetNetworkStatus(bool active)
        {
            this.isNetworkActive = active;
        }
        public bool GetWifiStatus()
        {
            return isWifiActive;
        }
        public void SetWifiStatus(bool active)
        {
            this.isWifiActive = active;
        }

        public bool GetSyncStatus()
        {
            return synced;
        }
        public void SetSyncStatus(bool sync)
        {
            this.synced = sync;
        }

        public string GetDialogTitle()
        {
            return dialogTitle;
        }

        public void SetDialogTitle(string title)
        {
            this.dialogTitle = title;
        }

        public string GetDialogMsg()
        {
            return dialogMsg;
        }
        public void SetDialogMsg(string msg)
        {
            this.dialogMsg = msg;
        }

        public DateTime GetAppActiveDate()
        {
            return lastActiveDate;
        }
        private void SetAppActiveDate(DateTime dt)
        {
            this.lastActiveDate = dt;
        }

		public Assignment GetLastSelectedAssignment()
		{
			return lastassignment;
		}
		public void SetLastSelectedAssignment(Assignment assignment)
		{
			this.lastassignment = assignment;
		}
       
		public int GetLastFragmentId()
		{
			return lastFragmentId;
		}
		public void SetLastFragmentId(int id)
		{
			this.lastFragmentId = id;
		}

		public int GetLastBaseContainId()
		{
			return lastBaseContainerID;
		}
		public void SetLastBaseContainId(int id)
		{
			this.lastBaseContainerID = id;
		}

		public string GetLastSelectedQueueName()
		{
			return lastSelectedQueueName;
		}
		public void SetLastSelectedQueueName(string mi)
		{
			this.lastSelectedQueueName = mi;
		}

		public MenuItem GetLastSelectedMenu()
		{
			return lastSelectedMenu;
		}
		public void SetLastSelectedMenu(MenuItem mi)
		{
			this.lastSelectedMenu = mi;
		}
        #endregion

        public void InvokeBaseAlertDialog(string title, string msg)
        {
            var dlgAlert = (new AlertDialog.Builder(this)).Create();
            dlgAlert.SetMessage(title);
            dlgAlert.SetTitle(msg);
            dlgAlert.SetButton("Dismiss", dismissDialog);
            dlgAlert.Show();

        }

        private void dismissDialog(object sender, DialogClickEventArgs e)
        {
            AlertDialog objAlertDialog = sender as AlertDialog;
            objAlertDialog.Dismiss();
        }

        public bool ShouldSyncApplicationDate()
        {
            bool shouldSync = false;
            DateTime lastDate = GetAppActiveDate();
            if (lastDate.Date.CompareTo(DateTime.Today.Date) != 0)
            {
                shouldSync = true;
            }
            return shouldSync;
        }

		private static ApplicationShared current;
		public static ApplicationShared Current{
			get { return current; }
		}

	    static ApplicationShared(){
			current = new ApplicationShared ();
		}

		public SyncService SyncService
		{
			get {
				if (this._serviceConnection.Binder == null)
					throw new Exception ("Service not bound yet");
				// note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
				return this._serviceConnection.Binder.Service;
			}
		}

		public event EventHandler<ServiceConnectedEventArgs> SyncServiceConnected = delegate {};
		protected ServiceConnection _serviceConnection;
		#region background services
		protected ApplicationShared(){
			new Task (() => {
				Intent syncServiceIntent = new Intent (global::Android.App.Application.Context, typeof(SyncService));
				global::Android.App.Application.Context.BindService (syncServiceIntent, _serviceConnection, Bind.AutoCreate);
			}).Start ();
		}


		#endregion
    }
}
