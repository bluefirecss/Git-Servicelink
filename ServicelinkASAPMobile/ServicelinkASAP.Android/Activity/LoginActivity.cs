
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

/// <summary>
/// not using this activity
/// </summary>
namespace ServicelinkASAP.Android
{
	[Activity (Label = "LoginActivity")]			
	public class LoginActivity : Activity, TextView.IOnEditorActionListener
	{
		EditText password, userName;
		Button login;
		ProgressBar prograssbar;
		TextView errorLogin;
		User user = new User();
		AccountDataAccess ada = new AccountDataAccess();
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our controls from the layout resource,
			// and attach an event to it
			login = FindViewById<Button> (Resource.Id.loginButton);
			userName = FindViewById<EditText> (Resource.Id.textUserName);
			password = FindViewById<EditText> (Resource.Id.txtPassword);
			prograssbar = FindViewById<ProgressBar> (Resource.Id.progressBar1);
			errorLogin = FindViewById<TextView> (Resource.Id.textLoginError);
			//var loginHelp = FindViewById<ImageButton> (Resource.Id.loginQuestion);

			//Set edit action listener to allow the next & go buttons on the input keyboard to interact with login.
			userName.SetOnEditorActionListener (this);
			password.SetOnEditorActionListener (this);


			userName.TextChanged += (sender, e) => {
				user.UserName = userName.Text;
			};
			password.TextChanged += (sender, e) => {
				user.Password = password.Text;
			};
			/*
			loginHelp.Click += (sender, e) => {
				var builder = new AlertDialog.Builder (this)
					.SetTitle ("Need Help?")
					.SetMessage ("Enter any username or password.")
					.SetPositiveButton ("Ok", (innerSender, innere) => { });
				var dialog = builder.Create ();
				dialog.Show ();
			};*/

			//initially set username & login to set isvalid on the view model.
			user.UserName = userName.Text;
			user.Password = password.Text;
			user.DeviceType = ProjConfiguration.deviceType;
			user.AppVersion = ProjConfiguration.appVersion;
			//LogIn button click event
			login.Click += (sender, e) => Login ();

			//request focus to the edit text to start on username.
			userName.RequestFocus ();
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
			if (!string.IsNullOrEmpty (userName.Text) && !string.IsNullOrEmpty (password.Text)) {
				//this hides the keyboard
				var imm = (InputMethodManager)GetSystemService (Context.InputMethodService);
				imm.HideSoftInputFromWindow (password.WindowToken, HideSoftInputFlags.NotAlways);
				login.Visibility = ViewStates.Invisible;
				prograssbar.Visibility = ViewStates.Visible;

				bool succeed = LoginWebService.LoginAsync (user.UserName, user.Password, user.DeviceType, user.AppVersion);
				if (succeed) {
					ada.SaveUserAsync (user);
				} else {
					login.Visibility = ViewStates.Visible;
					prograssbar.Visibility = ViewStates.Invisible;
					errorLogin.Text = "Invalid UserName Or Password.";
				}
			}
		}
	}
}

