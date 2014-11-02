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
using Android.Net;
using Android.Provider;
using Android.Graphics;
using Android.Views.Animations;
using Android.Service;
using Java.IO;
using Android.Locations;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
	public static class App{
		public static File _file;
		public static File _dir;     
		public static Bitmap bitmap;
		public static string OrderID;
		public static Location currentLocation;
		public static bool locationServiceEnabled;
	}

	[Activity (Label = "ServicelinkASAP.Android",ConfigurationChanges=(global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.ScreenSize), MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, ILocationListener
	{
		int baseFragment;
		bool userAuthenticated = false;
		Button menuButton = null;
		SliderMenu _menu;
        Http serviceLinkWS = new Http();
        PostingDataAccess pda = new PostingDataAccess();
        ConnectivityManager connectivityManager;
        //ApplicationShared app;
        ExpandableListViewActivity menuExpandableList;
		private AssignmentListViewFragment _listFragment;
		private PostingDetailFragment _postingDetailFragment;
		private SalesDetailFragment _saleDetailFragment;
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //app = (ApplicationShared)Application.ApplicationContext;           
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            var menu = FindViewById<SliderMenu>(Resource.Id.SliderMenu);
            menuButton = (Button)FindViewById(Resource.Id.MenuButton);
            menuButton.Click += (sender, e) =>
            {
                menu.AnimatedOpened = !menu.AnimatedOpened;
                UpdateMenuItems();
            };
            _menu = menu;

            if (FragmentManager.BackStackEntryCount == 0)
            {
                userAuthenticated = false;
            }
            else
            {
                userAuthenticated = checkUserAuthentication();
            }
			InitializeLocationManager ();
            IsNetworkConnected();
            IsWifiConnected();
            LoadDefaultView();
        }

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			outState.PutInt ("baseFragment", baseFragment);
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			baseFragment = savedInstanceState.GetInt ("baseFragment");
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			SetupActionBar (FragmentManager.BackStackEntryCount != 0);
		}

		public override bool OnMenuItemSelected (int featureId, IMenuItem item)
		{
			return base.OnMenuItemSelected (featureId, item);
		}
			

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case  global::Android.Resource.Id.Home:
				FragmentManager.PopBackStack (baseFragment, PopBackStackFlags.Inclusive);
				SetupActionBar ();
				return true;
			
			}
			return base.OnOptionsItemSelected (item);
		}
			
		public bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);         
			IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			_locationManager.RemoveUpdates (this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (_locationManager == null) {
				InitializeLocationManager ();
			}
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
		}

		public string GetStringFromPeviousActivity(string dataName){
			string text = Intent.GetStringExtra (dataName) ?? "Data not available";

			return text;
		}

		public void PassDataToNextActivity(Activity activity, string dataName, string data){
			var intent = new Intent (this, typeof(MainActivity));
			intent.PutExtra (dataName, data);
			//StartActivity (intent);
		}
      
		public override void OnConfigurationChanged (global::Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
		}

		#region location method
		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = String.Empty;
			}
		}

		public void OnLocationChanged(Location location) {
			_currentLocation = location;
			if (_currentLocation != null)
			{
				App.currentLocation = location;
			}
		}

		public void OnProviderDisabled(string provider) {
			App.locationServiceEnabled = false;
		}

		public void OnProviderEnabled(string provider) {App.locationServiceEnabled = true;}

		public void OnStatusChanged(string provider, Availability status, Bundle extras) {}

		#endregion


		#region Custom methods
	
		public bool IsNetworkConnected()
		{ 
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected)
			{
				ApplicationShared.Current.SetNetworkStatus(true);
				return true;
			}
			else
			{
				ApplicationShared.Current.SetNetworkStatus(false);
				return false;
			}
		}

		public bool IsWifiConnected()
		{
			var mobileState = connectivityManager.GetNetworkInfo(ConnectivityType.Mobile).GetState();
			if (mobileState == NetworkInfo.State.Connected)
			{
				ApplicationShared.Current.SetWifiStatus(true);
				return true;
			}
			else
			{
				ApplicationShared.Current.SetWifiStatus(false);
				return false;
			}
		}

		/// <summary>
		/// Setups the action bar if we want to show up arrow or not
		/// </summary>
		/// <param name="showUp">If set to <c>true</c> show up.</param>
		public void SetupActionBar (bool showUp = false)
		{
			this.ActionBar.SetDisplayHomeAsUpEnabled (showUp);

		}

		public void SyncData()
		{
			List<Posting> postings = new List<Posting>();
			serviceLinkWS.GetPostings().ContinueWith(t =>
				{
					postings = t.Result;

				});

			if (postings.Count > 0)
			{
				foreach (Posting p in postings)
				{
					pda.SavePosting_Async(p);
				}
			}
		}

		public void UpdateMenuItems(){
			if (menuExpandableList == null)
			{
				menuExpandableList = new ExpandableListViewActivity();
				menuExpandableList.MenuItemSelected += showMenuSelectedQueue;
				var intent = new Intent(this, typeof(ExpandableListViewActivity));
				StartActivity(intent);
			}
		}


		public int SwitchScreens (Fragment fragment, bool animated = true, bool isdetail = false, bool isRoot = false)
		{
			int baseContainer = Resource.Id.ContentView;
			ApplicationShared.Current.SetLastBaseContainId (baseContainer);
			if (!isRoot) {
				baseContainer = Resource.Id.mainContainer;
				if (isdetail) {
					baseContainer = FindViewById (Resource.Id.detailContainer) != null ? Resource.Id.detailContainer : Resource.Id.mainContainer;
				}
			}
			var transaction = FragmentManager.BeginTransaction ();

			if (animated) {
				transaction.SetCustomAnimations (
					enter: Resource.Animator.slide_in_left,
					exit: Resource.Animator.slide_out_left,
					popEnter: Resource.Animator.slide_in_right,
					popExit: Resource.Animator.slide_out_right);
			}
			transaction.Replace (baseContainer, fragment);
			if (!isRoot) {
				transaction.AddToBackStack (null);
			} else {
				ApplicationShared.Current.SetLastFragmentId (fragment.Id);
			}

			SetupActionBar (!isRoot);
			transaction.SetTransition (FragmentTransit.FragmentFade);
			return transaction.Commit ();
		}

		public void RefreshCurrentScreen(Fragment currentFragment){
			var transaction = FragmentManager.BeginTransaction ();
			transaction.Detach(currentFragment);
			transaction.Attach(currentFragment);
			transaction.Commit();
		}

		public void BackToListView(){
			//var orderListFragment = new AssignmentListViewFragment ();
			if (FindViewById (Resource.Id.detailContainer) != null) {
				baseFragment = _listFragment.Id;
				SwitchScreens (_listFragment, true, false, false);
			}
		}

		public void ReceiveAssignment(){
			RunOnUiThread (()=>{

			});
		}

		public void showMenuSelectedQueue(MenuItem mItem, string queueName)
		{

			if (queueName != "Account") {
				if (_listFragment == null) {
					_listFragment = new AssignmentListViewFragment (mItem, queueName);
				}
				baseFragment = _listFragment.Id;
				_listFragment.RefreshList += SyncData;
				_listFragment.AssignmentSelected += ShowAssignmentDetail;
				//if base view is split view, notify AssignmentListView that data source has changed, no need to switch views
				if (ApplicationShared.Current.GetLastSelectedQueueName() != queueName) { //last selected queue isn't listView
					SwitchScreens (_listFragment, true, false, false);

				} else {
					RefreshCurrentScreen (_listFragment);
				}
			}
			else
			{
				Fragment newFragment = new LoginFragment ();
				if (mItem.Name == "Signature") {
					newFragment = new SignatureFragment ();
				} else if (mItem.Name == "User Info") {
					newFragment = new AccountFragment ();
				}
				baseFragment = newFragment.Id;
				if (ApplicationShared.Current.GetLastSelectedQueueName() != queueName) {
					SwitchScreens(newFragment, true, false, true);
				} else {
					if (ApplicationShared.Current.GetLastSelectedMenu ().Name != mItem.Name) {
						SwitchScreens(newFragment, true, false, true);
					} else {
						RefreshCurrentScreen (newFragment);
					}
				}
			}

			ApplicationShared.Current.SetLastSelectedMenu (mItem);
			ApplicationShared.Current.SetLastSelectedQueueName (queueName);
		}

		public void ShowAssignmentDetail(Assignment assignment)
		{
			ApplicationShared.Current.SetLastSelectedAssignment (assignment);
			if (assignment.Type == AssignmentType.Postings)
			{
				App.OrderID = assignment.posting.OrderID;
				_postingDetailFragment = new PostingDetailFragment(assignment.posting);
				_postingDetailFragment.appHasCamera = IsThereAnAppToTakePictures();
				baseFragment = _postingDetailFragment.Id;
				_postingDetailFragment.backToList += BackToListView;
				_postingDetailFragment.claimException += loadExceptionView;
				_postingDetailFragment.loadNextInQueue += loadNextInQueue;
				/*
				_postingDetailFragment.checkLocationCoordinate += delegate {
					_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
				};*/
				SwitchScreens(_postingDetailFragment, true, true, false);
			}
			else if (assignment.Type == AssignmentType.Sales)
			{
				App.OrderID = assignment.sale.OrderID;
				_saleDetailFragment = new SalesDetailFragment();
				SwitchScreens (_saleDetailFragment, true, true, false);
			}

		}

		public void loadOrderListView(){
			if (ApplicationShared.Current.GetNetworkActive())
			{
				SyncData();
			}
			else
			{
				ApplicationShared.Current.InvokeBaseAlertDialog("Sync Error", "There is no network connection, failed to sync data.");
			}

			//UpdateMenuItems ();
			//var intent = new Intent(this, typeof(AssignmentListViewActivity));
			//StartActivity(intent);

			var splitFragment = new SplitViewFragment ();
			splitFragment.LoadListViewFragment += LoadListViewFragment;
			baseFragment = splitFragment.Id;

			SwitchScreens (splitFragment, true, false, true);
		}

		private void LoadListViewFragment(){
			if (_listFragment == null) {
				_listFragment = new AssignmentListViewFragment ();
			}
			_listFragment.RefreshList += SyncData;
			_listFragment.AssignmentSelected += ShowAssignmentDetail;
			SwitchScreens (_listFragment, true, false, false);
		}



		public void ShowHideLeftMenu(){
			_menu.AnimatedOpened = !_menu.AnimatedOpened;
		}

		public bool checkUserAuthentication(){
			AccountDataAccess ada = new AccountDataAccess ();
			Boolean allowLogin = ada.AllowUserLogin ();
			return allowLogin;
		}

		public void LoadDefaultView(){
			if (userAuthenticated) {
				menuButton.Visibility = ViewStates.Visible;		
				loadOrderListView ();		
			} else {
				menuButton.Visibility = ViewStates.Invisible;
				loadLoginView ();
			}
		}

		public void loadLoginView(){
			//var intent = new Intent(this, typeof(LoginActivity));
			//StartActivity(intent);
			var login = new LoginFragment();
			login.LoginSucceeded += () =>
			{
				userAuthenticated = true;
				LoadDefaultView();
			};
			baseFragment = login.Id;
			SwitchScreens(login, true, false, true);
		}

		public void loadExceptionView(Posting posting){
			var exceptionFragment = new ExceptionFragment (posting);
			exceptionFragment.submitException += loadNextInQueue;
			baseFragment = exceptionFragment.Id;
			SwitchScreens(exceptionFragment, true, false, false);
		}

		private void loadNextInQueue(){
			string queue = ApplicationShared.Current.GetLastSelectedQueueName ();
			MenuItem item = ApplicationShared.Current.GetLastSelectedMenu ();
			Assignment assignment = new Assignment ();
			if (queue == "Postings") {
				List<Posting> p = new List<Posting> ();
				assignment.Type = AssignmentType.Postings;
				if (item.Name == "Pending") {
					pda.GetPendingPostings_Async ().ContinueWith (t => {
						p = t.Result;
						if (p.Count > 0) {
							assignment.posting = p.FirstOrDefault<Posting> ();
						}
					});
				}

				if (item.Name == "Complete") {
					pda.GetCompletedPostings_Async ().ContinueWith (t => {
						p = t.Result;
						if (p.Count > 0) {
							assignment.posting = p.FirstOrDefault<Posting> ();
						}
					});
				}

				if (item.Name == "Cancelled") {
					pda.GetCancelledPostings_Async ().ContinueWith (t => {
						p = t.Result;
						if (p.Count > 0) {
							assignment.posting = p.FirstOrDefault<Posting> ();
						}
					});
				}
			}

			if (queue == "Sales") {
				List<Sale> s = new List<Sale> ();
				assignment.Type = AssignmentType.Sales;
				if (item.Name == "Pending") {

				}

				if (item.Name == "Complete") {

				}

				if (item.Name == "Cancelled") {

				}

				if (item.Name == "Hold") {

				}

				if (item.Name == "Postponed") {

				}
			}

			ShowAssignmentDetail (assignment);
		}

	
		#endregion
	}


}


