
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicelinkASAPMobile.Utilities;

namespace ServicelinkASAPMobile.Data {
	/// <summary>
	/// An assignment is the "thing" or "job" the user is going to work on
	/// </summary>
    ///  
    [Serializable()]
	public class Posting {
		/// <summary>
		/// Assignment Id
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string OrderID { get; set; }
	
		public PostingStatus Status { get; set; }

		public string TSNum { get; set; }

		public string APN { get; set; }

		public string DatedDate { get; set; }

		public string SaleDateTime { get; set; }

        public string CancelInd { get; set; }

		public string ClientNum { get; set; }

		public string PostByDate { get; set; }

		public string PostOn { get; set; }

		public string PostType { get; set; }

		public string PostNotes { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
        public string IsRejected { get; set; }

        public string Trustor { get; set; }
        public string Completed { get; set; }
        public string Comments { get; set; }
        public string ForSaleBy { get; set; }
        public string ForSaleSign { get; set; }
        public string NOS { get; set; }
        public string NOR { get; set; }
        public string NOD { get; set; }
        public string Occupancy { get; set; }
        public string OccupantContacted { get; set; }
        public string PostingLocation { get; set; }

        public string PropertyType { get; set; }
        public string IsException { get; set; }
        public string Exception { get; set; }
        public string SyncUpDate { get; set; }
        public string SavedTimeStamp { get; set; }
        public string SyncFlag { get; set; }
        public string NumberOfPostings { get; set; }
        public string RePostDate { get; set; }

		public float Latitude { get; set; }

		public float Longitude { get; set; }

        public Photos[] Photos { get; set;}


		#region UI properties

		
		public string PostByDateFormatted
		{
			get
			{
                return Convert.ToDateTime(PostByDate).ToShortDateString();
			}
		}

		/// <summary>
		/// A formatted version of the times for WinRT
		/// </summary>
        //public string TimesFormatted
        //{
        //    get
        //    {
        //        return Convert.ToDateTime(SaleDateTime).ToShortTimeString() +
        //            Environment.NewLine +
        //            "· · ·" +
        //            Environment.NewLine +
        //            EndDate.ToShortTimeString();
        //    }
        //}

		/// <summary>
		/// A formatted version of the start time for WinRT
		/// </summary>
		/*public string StartTimeFormatted
		{
			//get { return StartDate.ToShortTimeString (); }
		}*/

		/// <summary>
		/// A formatted version of the end time for WinRT
		/// </summary>
		/*public string EndTimeFormatted
		{
			//get { return EndDate.ToShortTimeString (); }
		}*/

		/// <summary>
		/// A formatted version of the address for WinRT
		/// </summary>
		public string AddressFormatted
		{
			get
			{
				return Address +
					Environment.NewLine +
					City + ", " + State + " " + Zip;
			}
		}

		/// <summary>
		/// Formatted version of total hours for WinRT
		/// </summary>
		/*public string TotalHoursFormatted
		{
			//get { return TotalHours.TotalHours.ToString ("0.0"); }
		}*/

		/// <summary>
		/// Formatted version of expenses for WinRT
		/// </summary>
		/*public string TotalExpensesFormatted
		{
			//get { return TotalExpenses.ToString ("$0.00"); }
		}*/

		/// <summary>
		/// If true, this assignment is not editable - it is complete or a history record
		/// </summary>
		
		/// <summary>
		/// If true, it's ok to complete this assignment
		/// </summary>
        //public bool CanComplete
        //{
        //    get { return Status != AssignmentStatus.New && !IsReadonly; }
        //}

		#endregion

        public string GetJson(Posting properties)
        {
            OrderID = properties.OrderID;
            TSNum = properties.TSNum;
            APN = properties.APN;
            Photos = properties.Photos;
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
	}
}