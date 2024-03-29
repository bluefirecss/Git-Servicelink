using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Graphics;
using Android.Net;
using Android.Provider;
using Android.Widget;
using Java.IO;
using Android.Runtime;
using Android.Util;
using Android.Views;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Utilities;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP.Android
{
    public class PostingDetailFragment : Fragment
    {
		public event Action backToList = delegate {};
		public event Action<Posting> claimException = delegate {};
		public event Action loadNextInQueue = delegate {};
		//public event Action checkLocationCoordinate = delegate{};
		public bool appHasCamera;

		List<Photo> photos = new List<Photo>();
		Posting posting = null;
		LayoutInflater inflater;
		Gallery gallery = null;
		PostingDataAccess pda = new PostingDataAccess();
		PhotoDataAccess photoda = new PhotoDataAccess ();
		//ApplicationShared app;
		TextView txtOrderID;
		TextView txtTS;
		TextView txtAPN;
		TextView txtPostOn;
		Button btnAddress;
		RadioButton rdbFront;
		RadioButton rdbGate ;
		RadioButton rdbOther ;
		RadioButton rdbOccupied ;
		RadioButton rdbUnoccupied ;
		RadioButton rdbUnknown ;
		RadioButton rdbVacant ;
		RadioButton rdbResidential;
		RadioButton rdbCommercial ;
		RadioButton rdbRural ;
		RadioButton rdbNOS ;
		RadioButton rdbNOD;
		RadioButton rdbContactYes ;
		RadioButton rdbContactNo ;
		RadioButton rdbSignYes;
		RadioButton rdbSignNo;
		CheckBox chkNTR ;
		EditText etxtSaleBy;
		EditText etxtPostingNum;
		EditText etxtComment;
		ImageButton btnSave ;
		int imgWidth = 100;

		public PostingDetailFragment(){
		}

		public PostingDetailFragment(Posting p){
			posting = p;
		}

		public override void OnStart ()
		{
			base.OnStart ();
		}

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			RetainInstance = true;
			SetHasOptionsMenu(true);

			if (posting == null) {
				posting = ApplicationShared.Current.GetLastSelectedAssignment ().posting;
			}

			if (appHasCamera) {
				CreateDirectoryForPictures ();
			}
		
        }

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			this.inflater = inflater;
			return inflater.Inflate(Resource.Layout.PostingDetailLayout, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{	
			txtOrderID = view.FindViewById<TextView> (Resource.Id.txtOrderID_P);
			txtTS = view.FindViewById<TextView> (Resource.Id.txtTSNum_P);
			txtAPN = view.FindViewById<TextView> (Resource.Id.txtAPN_P);
			txtPostOn = view.FindViewById<TextView> (Resource.Id.txtPostOn);
			btnAddress = view.FindViewById<Button> (Resource.Id.btnAddress_P);
			rdbFront = view.FindViewById<RadioButton> (Resource.Id.rdbFront);
			rdbGate = view.FindViewById<RadioButton> (Resource.Id.rdbGate);
			rdbOther = view.FindViewById<RadioButton> (Resource.Id.rdbOther);
			rdbOccupied = view.FindViewById<RadioButton> (Resource.Id.rdbOccupied);
			rdbUnoccupied = view.FindViewById<RadioButton> (Resource.Id.rdbUnoccupied);
			rdbUnknown = view.FindViewById<RadioButton> (Resource.Id.rdbUnknown);
			rdbVacant = view.FindViewById<RadioButton> (Resource.Id.rdbVacant);
			rdbResidential = view.FindViewById<RadioButton> (Resource.Id.rdbResidential);
			rdbCommercial = view.FindViewById<RadioButton> (Resource.Id.rdbCommercial);
			rdbRural = view.FindViewById<RadioButton> (Resource.Id.rdbRural);
			rdbNOS = view.FindViewById<RadioButton> (Resource.Id.rdbNOS);
			rdbNOD = view.FindViewById<RadioButton> (Resource.Id.rdbNOD);
			rdbContactYes = view.FindViewById<RadioButton> (Resource.Id.rdbContact_yes);
			rdbContactNo = view.FindViewById<RadioButton> (Resource.Id.rdbContact_no);
			rdbSignYes = view.FindViewById<RadioButton> (Resource.Id.rdbSalesign_yes);
			rdbSignNo = view.FindViewById<RadioButton> (Resource.Id.rdbSalesign_no);
			chkNTR = view.FindViewById<CheckBox> (Resource.Id.chkNTR);
			etxtSaleBy = view.FindViewById<EditText> (Resource.Id.txtSaleBy);
			etxtPostingNum = view.FindViewById<EditText> (Resource.Id.txtNumPostings);
			etxtComment = view.FindViewById<EditText> (Resource.Id.txtComment);

			rdbFront.Click += RadioButtonClick;
			rdbGate.Click += RadioButtonClick;
			rdbOther.Click += RadioButtonClick;

			rdbOccupied.Click += RadioButtonClick;
			rdbUnoccupied.Click += RadioButtonClick;
			rdbUnknown.Click += RadioButtonClick;
			rdbVacant.Click += RadioButtonClick;

			rdbResidential.Click += RadioButtonClick;
			rdbCommercial.Click += RadioButtonClick;
			rdbRural.Click += RadioButtonClick;

			rdbContactYes.Click += RadioButtonClick;
			rdbContactNo .Click += RadioButtonClick;
			rdbSignYes .Click += RadioButtonClick;
			rdbSignNo .Click += RadioButtonClick;

			rdbNOS.Click += RadioButtonClick;
			rdbNOD.Click += RadioButtonClick;
			chkNTR.Click += CheckBoxClick;

			Button removeImageButton = view.FindViewById<Button> (Resource.Id.clearImgButton);
			removeImageButton.Click += ImageButtonClick;

			ImageButton btnCamera = view.FindViewById<ImageButton> (Resource.Id.btnCamera);
			ImageButton btnClear = view.FindViewById<ImageButton> (Resource.Id.btnClear);
			btnSave = view.FindViewById<ImageButton> (Resource.Id.btnSave);
			ImageButton btnException = view.FindViewById<ImageButton> (Resource.Id.btnException);

			btnCamera.Click += TakeAPicture;
			btnSave.Click += SavePosting;
			btnClear.Click += ResetPage;
			btnException.Click += OpenException;

			DetectLocationService ();

			LoadOrderDetails ();
			GetPhotos ();
			//load images in gallery
			gallery = (Gallery)view.FindViewById<Gallery> (Resource.Id.gallery1);
			ImageAdapter adapter = new ImageAdapter (view.Context, photos);
			gallery.Adapter = adapter;
			adapter.NotifyDataSetChanged ();

			gallery.ItemClick += delegate (object _sender, AdapterView.ItemClickEventArgs args) {
				View popupView = inflater.Inflate (Resource.Layout.ImageLayout, null);
				PopupWindow popup = new PopupWindow (popupView, WindowManagerLayoutParams.WrapContent, WindowManagerLayoutParams.WrapContent);

				Button dismissButton = popupView.FindViewById<Button> (Resource.Id.btnClosePopup);
				dismissButton.Click += (sender, e) => {
					popup.Dismiss ();
				};
				popup.ShowAtLocation (view, GravityFlags.Center, 0, 0);
				//Toast.MakeText (this, args.Position.ToString (), ToastLength.Short).Show ();
			};
		}

		public override void OnResume ()
		{
			base.OnResume ();
			DetectLocationService ();
		}

		private void DetectLocationService(){
			if (btnSave == null) {
				btnSave = this.View.FindViewById<ImageButton> (Resource.Id.btnSave);
			}
			btnSave.Enabled = true;
			if (!App.locationServiceEnabled) {
				btnSave.Enabled = false;
				var dlgAlert = (new AlertDialog.Builder (this.View.Context)).Create ();
				dlgAlert.SetMessage ("Location service is currently disabled in your device, you will not be able to SAVE normal files until the service is enabled!");
				dlgAlert.SetTitle ("Application Warning");
				dlgAlert.SetButton ("Dismiss", (object sender, DialogClickEventArgs args) =>{
					dlgAlert.Dismiss();
				});

				dlgAlert.Show ();
			}
		}


		private void RadioButtonClick (object sender, EventArgs e)
		{
			RadioButton rb = (RadioButton)sender;
			if (rb.Id == 2131296298) { //front door
				checkRadioButton_PostingLocation (PostingLocation.FrontDoor);
			}
			if (rb.Id == 2131296299) { //gate
				checkRadioButton_PostingLocation (PostingLocation.Gate);
			}
			if (rb.Id == 2131296300) { //other
				checkRadioButton_PostingLocation (PostingLocation.Other);
			}

			if (rb.Id == 2131296309) { //residential
				checkRadioButton_PropertyType (PropertyType.Residential);
			}
			if (rb.Id == 2131296310) { //commercial
				checkRadioButton_PropertyType (PropertyType.Commercial);
			}
			if (rb.Id == 2131296311) { //rural
				checkRadioButton_PropertyType (PropertyType.Rural);
			}

			if (rb.Id == 2131296303) { //occupied
				checkRadioButton_Occupancy (Occupancy.Occupied);
			}
			if (rb.Id == 2131296304) { //unoccupied
				checkRadioButton_Occupancy (Occupancy.Unoccupied);
			}
			if (rb.Id == 2131296305) { //unknown
				checkRadioButton_Occupancy (Occupancy.Unknown);
			}
			if (rb.Id == 2131296306) { //vacant
				checkRadioButton_Occupancy (Occupancy.VacantLand);
			}

			if (rb.Id == 2131296324) {
				checkRadioButton_Contact (true);
			}
			if (rb.Id == 2131296325) {
				checkRadioButton_Contact (false);
			}

			if (rb.Id == 2131296329) {
				checkRadioButton_SaleSign (true);
			}
			if (rb.Id == 2131296330) {
				checkRadioButton_SaleSign (false);
			}

			if (rb.Id == 2131296317) {
				checkRadioButton_NOS (true);
			}
			if (rb.Id == 2131296318) {
				checkRadioButton_NOS (false);
			}
			//Toast.MakeText (this, rb.Text, ToastLength.Short).Show ();
		}

		private void CheckBoxClick (object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;
			checkRadioButton_NOS (cb.Checked);
		}

		private void ImageButtonClick (object sender, EventArgs e)
		{
			var dlgAlert = (new AlertDialog.Builder (this.View.Context)).Create ();
			dlgAlert.SetMessage ("Are you sure to delete all images for this file?");
			dlgAlert.SetTitle ("Confirm Delete");
			dlgAlert.SetButton ("Continue", DeletePhotoDialogHandler);
			dlgAlert.SetButton2 ("Cancel", DeletePhotoDialogHandler);
			dlgAlert.Show ();
		}

		private void TakeAPicture(object sender, EventArgs args)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);

			App._file = new File(App._dir, String.Format("{0}_{1}.jpg", App.OrderID , Guid.NewGuid()));

			intent.PutExtra(MediaStore.ExtraOutput, global::Android.Net.Uri.FromFile(App._file));

			StartActivityForResult(intent, 0);
		}

		private void ResetPage(object sender, EventArgs args)
		{
			var dlgAlert = (new AlertDialog.Builder (this.View.Context)).Create ();
			dlgAlert.SetMessage ("Are you sure to clear the user input?");
			dlgAlert.SetTitle ("Confirm Clear");
			dlgAlert.SetButton ("Continue", ClearDialogHandler);
			dlgAlert.SetButton2 ("Cancel", ClearDialogHandler);
			//dlgAlert.SetButton3 ("Nothing", handllerNotingButton);
			dlgAlert.Show ();
		}

		private void SavePosting(object sender, EventArgs args)
		{
			var dlgAlert = (new AlertDialog.Builder (this.View.Context)).Create ();
			dlgAlert.SetMessage ("Do you certifiy that the above is true?");
			dlgAlert.SetTitle ("Confirm Save");
			dlgAlert.SetButton ("Yes", SaveDialogHandler);
			dlgAlert.SetButton2 ("No", SaveDialogHandler);
			dlgAlert.Show ();
		}

		private void OpenException(object sender, EventArgs args)
		{
			claimException (posting);
		}

		private void ClearDialogHandler(object sender, DialogClickEventArgs e){
			AlertDialog objAlertDialog = sender as AlertDialog;
			Button btnClicked = objAlertDialog.GetButton (e.Which);
			if (btnClicked.Text == "Continue") {
				ClearInput ();
			}
			if (btnClicked.Text == "Cancel") {
				objAlertDialog.Dismiss ();
			}
			//Toast.MakeText (this, "you clicked on " + btnClicked.Text, ToastLength.Long).Show ();
		}

		private void SaveDialogHandler(object sender, DialogClickEventArgs e){
			AlertDialog objAlertDialog = sender as AlertDialog;
			Button btnClicked = objAlertDialog.GetButton (e.Which);
			if (btnClicked.Text == "Yes") {
				SaveOrder ();
			}
			if (btnClicked.Text == "No") {
				objAlertDialog.Dismiss ();
			}

		}

		private void DeletePhotoDialogHandler(object sender, DialogClickEventArgs e){
			AlertDialog objAlertDialog = sender as AlertDialog;
			Button btnClicked = objAlertDialog.GetButton (e.Which);
			if (btnClicked.Text == "Continue") {
				photoda.DeletePhotoList_Async (photos);
				ImageAdapter adapter = (ImageAdapter)gallery.Adapter;
				adapter.NotifyDataSetChanged ();
			}
			if (btnClicked.Text == "Cancel") {
				objAlertDialog.Dismiss ();
			}
		}

		private void LoadOrderDetails(){
			if (posting != null) {
				txtOrderID.Text = posting.OrderID;
				txtTS.Text = posting.TSNum;
				txtAPN.Text = posting.APN;
				txtPostOn.Text = posting.PostOn;
				btnAddress.Text = posting.Address;
				checkRadioButton_PostingLocation (posting.PostingLocation);
				checkRadioButton_Occupancy (posting.Occupancy);
				checkRadioButton_PropertyType (posting.PropertyType);
				checkRadioButton_Contact (Convert.ToBoolean (posting.OccupantContacted == string.Empty ? "false" : posting.OccupantContacted));
				checkRadioButton_SaleSign (Convert.ToBoolean (posting.ForSaleSign == string.Empty ? "false" : posting.ForSaleSign));
				checkRadioButton_NOS (Convert.ToBoolean(posting.NOS == string.Empty? "false": posting.NOS));
				etxtSaleBy.Text = posting.ForSaleBy;
				etxtPostingNum.Text = posting.NumberOfPostings;
				etxtComment.Text = posting.Comments;
			} else {

				backToList ();
			}
		}

		private void checkRadioButton_PostingLocation(PostingLocation location){
			posting.PostingLocation = location;
			if (location == PostingLocation.FrontDoor) {
				rdbFront.Checked = true;
				rdbGate.Checked = false;
				rdbOther.Checked = false;
			}
			if (location == PostingLocation.Gate) {
				rdbFront.Checked = false;
				rdbGate.Checked = true;
				rdbOther.Checked = false;
			}
			if (location == PostingLocation.Other) {
				rdbFront.Checked = false;
				rdbGate.Checked = false;
				rdbOther.Checked = true;
			}
		}

		private void checkRadioButton_PropertyType(PropertyType type){
			posting.PropertyType = type;
			if (type == PropertyType.Residential) {
				rdbResidential.Checked = true;
				rdbCommercial.Checked = false;
				rdbRural.Checked = false;
			}
			if (type == PropertyType.Commercial) {
				rdbResidential.Checked = false;
				rdbCommercial.Checked = true;
				rdbRural.Checked = false;
			}
			if (type == PropertyType.Rural) {
				rdbResidential.Checked = false;
				rdbCommercial.Checked = false;
				rdbRural.Checked = true;
			}
		}

		private void checkRadioButton_Occupancy(Occupancy occu){
			posting.Occupancy = occu;
			if (occu == Occupancy.Occupied) {
				rdbOccupied.Checked = true;
				rdbUnoccupied.Checked = false;
				rdbUnknown.Checked = false;
				rdbVacant.Checked = false;
			}
			if (occu == Occupancy.Unoccupied) {
				rdbOccupied.Checked = false;
				rdbUnoccupied.Checked = true;
				rdbUnknown.Checked = false;
				rdbVacant.Checked = false;
			}
			if (occu == Occupancy.Unknown) {
				rdbOccupied.Checked = false;
				rdbUnoccupied.Checked = false;
				rdbUnknown.Checked = true;
				rdbVacant.Checked = false;
			}
			if (occu == Occupancy.VacantLand) {
				rdbOccupied.Checked = false;
				rdbUnoccupied.Checked = false;
				rdbUnknown.Checked = false;
				rdbVacant.Checked = true;
			}
		}

		private void checkRadioButton_Contact(bool canContact){
			posting.OccupantContacted = canContact.ToString();
			if (canContact) {
				rdbContactYes.Checked = true;
				rdbContactNo.Checked = false;
			} else {
				rdbContactYes.Checked = false;
				rdbContactNo.Checked = true;
			}

		}
		private void checkRadioButton_SaleSign(bool hasSign){
			posting.ForSaleSign = hasSign.ToString();
			if (hasSign) {
				rdbSignYes.Checked = true;
				rdbSignNo.Checked = false;
			} else {
				rdbSignYes.Checked = false;
				rdbSignNo.Checked = true;
			}

		}

		private void checkRadioButton_NOS(bool isNos){
			posting.NOD = (isNos ? false : true).ToString();
			posting.NOS = isNos.ToString();
			posting.NOR = isNos.ToString();
			if (isNos) {
				rdbNOS.Checked = true;
				rdbNOD.Checked = false;
				chkNTR.Checked = true;
			} else {
				rdbNOS.Checked = false;
				rdbNOD.Checked = true;
				chkNTR.Checked = false;
			}

		}

		private void GetOrderDetail(){

				pda.GetPostByID_Async (App.OrderID).ContinueWith (t => {
				posting = new Posting();
				List<Posting> p = t.Result;
				if(p.Count > 0){
					posting = p.FirstOrDefault<Posting>();

				}
			});
		}

		private void GetPhotos(){
			photoda.GetPhotos_Async (App.OrderID).ContinueWith (t => {
				photos = t.Result;
				posting.Photos = photos.ToArray();
			});
		}

		private void SaveOrder(){
			GetPhotos ();
			posting.Comments = etxtComment.Text.Trim ();
			posting.NumberOfPostings = etxtPostingNum.Text.Trim ();
			posting.ForSaleBy = etxtSaleBy.Text.Trim ();
			posting.SaleDateTime = DateTime.Now.ToString();
			string errroMsg = ValidateFields ();
			if (errroMsg == string.Empty) {
				pda.SavePosting_Async (posting).ContinueWith (t => {
					if (t.IsCompleted) {
						loadNextInQueue ();
					} else {
						Toast.MakeText (this.View.Context, "Oops, this action failed.", ToastLength.Long).Show ();
					}
				});
			} else {

				var dlgAlert = (new AlertDialog.Builder (this.View.Context)).Create ();
				dlgAlert.SetMessage (errroMsg);
				dlgAlert.SetTitle ("Validation Error");
				dlgAlert.SetButton ("Dismiss", (object sender, DialogClickEventArgs args) =>{
					dlgAlert.Dismiss();
				});

				dlgAlert.Show ();
			}
		}

		private string ValidateFields(){
			return ValidationHandler.ValidatePostingFields (posting);

		}

		private void ClearInput(){

			rdbFront.Checked = false;
			rdbGate.Checked = false;
			rdbOther.Checked = false;
			rdbOccupied.Checked = false;
			rdbUnoccupied.Checked = false;
			rdbUnknown.Checked = false;
			rdbVacant.Checked = false;
			rdbResidential.Checked = false;
			rdbCommercial.Checked = false;
			rdbRural.Checked = false;
			rdbContactYes.Checked = false;
			rdbContactNo.Checked = false;
			rdbSignYes.Checked = false;
			rdbSignNo.Checked = false;
			rdbNOD.Checked = false;
			chkNTR.Checked = false;
			etxtSaleBy.Text = string.Empty;
			etxtPostingNum.Text = string.Empty;
			etxtComment.Text = string.Empty;
		}

		private void SavePhotoToDB(){

			Photo photo = new Photo ();
			photo.FileName = App._file.Path;
			photo.OrderID = txtOrderID.Text;
			photo.DateCreated = DateTime.Now.ToString ();
			photo.Latitude =  App.currentLocation.Latitude;
			photo.Longitude = App.currentLocation.Longitude;
			photoda.SavePhoto_Async (photo);
		}

		#region Camera function
        //private void LoadImagesInGallery(View view){
        //    if (gallery != null) {
        //        if (gallery.ChildCount > 0) {
        //            gallery.Adapter = null;
        //            Adapter adapter =  new ImageAdapter (view.Context, photos);
        //            gallery.Adapter = adapter;
        //        }
        //    }
        //}

		private void CreateDirectoryForPictures()
		{
			App._dir = new File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "ServicelinkASAPMobile_Android");
			if (!App._dir.Exists())
			{
				App._dir.Mkdirs();
			}
		}


		#endregion

		#region set option menu
		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{		
			inflater.Inflate (Resource.Menu.postingmenu, menu);
			base.OnCreateOptionsMenu (menu, inflater);
		}
		/*
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			//return base.OnOptionsItemSelected (item);
			switch (item.ItemId) {
			case Resource.Id.camera:
				TakeAPicture ();
				break;
			case Resource.Id.exception:
				claimException ();
				break;
			case Resource.Id.normal:
				SaveOrder ();
				break;
			case Resource.Id.Clear:
				{
					ClearInput ();
					//Intent intent = new Intent (Intent.ActionDial);
					//StartActivity (intent);
					break;
				}
			default:
				break;
			}

			return true;

		}
		*/
		#endregion


		public override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			// make it available in the gallery
			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			global::Android.Net.Uri contentUri = global::Android.Net.Uri.FromFile (App._file);
			mediaScanIntent.SetData(contentUri);

			App.bitmap = App._file.Path.LoadAndResizeBitmap (imgWidth, Resources.DisplayMetrics.HeightPixels);
			SavePhotoToDB ();
			ImageAdapter adapter = (ImageAdapter)gallery.Adapter;
			adapter.NotifyDataSetChanged ();
		}
    }
}