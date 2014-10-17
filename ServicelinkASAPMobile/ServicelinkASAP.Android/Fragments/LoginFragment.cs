using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Service;
using Android.InputMethodServices;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;


namespace ServicelinkASAP.Android
{
    public class LoginFragment : Fragment, TextView.IOnEditorActionListener
	{
		public event Action LoginSucceeded = delegate {};
		Context mContext;
		EditText password, userName;
		Button login;
		ProgressBar prograssbar;
		TextView errorLogin;
		User user = new User ();
		AccountDataAccess ada = new AccountDataAccess();
        ApplicationShared app;
		public LoginFragment ()
		{
		}

		public LoginFragment (Context context)
		{
			this.mContext = context;
		}

		/*
			// Get our button from the layout resource,
		// and attach an event to it
		Button button = FindViewById<Button> (Resource.Id.loginButton);
		ImageView imgIndicator = FindViewById<ImageView>(Resource.Id.imgIndicator);


		button.Click += delegate {
			var rotateAboutCenterAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.rotate_center);
			imgIndicator.Visibility = ViewStates.Visible;
			imgIndicator.StartAnimation(rotateAboutCenterAnimation);


		};*/


		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RetainInstance = true;
            app = (ApplicationShared)Application.Context;   
			// Create your fragment here
		}
			

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			return CreateLoginView (inflater, container, savedInstanceState);
		}

		View CreateLoginView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Login, null);

			login = view.FindViewById<Button> (Resource.Id.loginButton);
			userName = view.FindViewById<EditText> (Resource.Id.textUserName);
			password = view.FindViewById<EditText> (Resource.Id.txtPassword);
			prograssbar = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);
			errorLogin = view.FindViewById<TextView> (Resource.Id.textLoginError);

			userName.SetOnEditorActionListener (this);
			password.SetOnEditorActionListener (this);

			user.UserName = userName.Text;
			user.Password = password.Text;
			user.DeviceType = ProjConfiguration.deviceType;
			user.AppVersion = ProjConfiguration.appVersion;

			userName.TextChanged += (sender, e) => {
				user.UserName = userName.Text;
			};
			password.TextChanged += (sender, e) => {
				user.Password = password.Text;
			};

			login.Click += (object sender, EventArgs e) => {
				Login();
			};

			userName.RequestFocus ();
			return view;
		}

		public bool OnEditorAction (TextView v, ImeAction actionId, KeyEvent e)
		{
           
			//go edit action will login
			if (actionId == ImeAction.Go) {
				if (!string.IsNullOrEmpty (userName.Text) && !string.IsNullOrEmpty (password.Text)) {
					Login ();
				} else if (string.IsNullOrEmpty (userName.Text)) {
					userName.RequestFocus ();
				} else {
					password.RequestFocus ();
				}
				return true;
				//next action will set focus to password edit text.
			} else if (actionId == ImeAction.Next) {
				if (!string.IsNullOrEmpty (userName.Text)) {
					password.RequestFocus ();
				}
				return true;
			}
			return false;
		}


		private void Login ()
		{
            if (app.GetNetworkActive() == false)
            {
                Toast.MakeText(mContext, "No Network Connection", ToastLength.Long).Show();
                return;
            }
			if (!string.IsNullOrEmpty (userName.Text) && !string.IsNullOrEmpty (password.Text)) {
				//this hides the keyboard
				if (mContext != null) {
					var imm = (InputMethodManager)mContext.GetSystemService (Context.InputMethodService);
					imm.HideSoftInputFromWindow (password.WindowToken, HideSoftInputFlags.NotAlways);
				}
				login.Visibility = ViewStates.Invisible;
				prograssbar.Visibility = ViewStates.Visible;

				bool succeed = LoginWebService.LoginAsync (user.UserName, user.Password, user.DeviceType, user.AppVersion);
				if (succeed) {
					ada.SaveUserAsync (user);
					LoginSucceeded ();
				} else {
					login.Visibility = ViewStates.Visible;
					prograssbar.Visibility = ViewStates.Invisible;
					errorLogin.Text = "Invalid UserName Or Password.";
				}
			}
		}

	}
}

