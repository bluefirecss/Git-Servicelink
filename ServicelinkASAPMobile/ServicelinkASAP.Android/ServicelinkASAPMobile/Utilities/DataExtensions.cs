using System;
using System.Globalization;
using ServicelinkASAPMobile.Data;

namespace ServicelinkASAPMobile.Utilities
{
	public static class DataExtensions {
	/*	public static string ToUserString (this LaborType type)
		{
			switch (type) {
			case LaborType.Hourly:
				return Catalog.GetString ("LaborTypeHourly");
			case LaborType.OverTime:
				return Catalog.GetString ("LaborTypeOverTime");
			case LaborType.HolidayTime:
				return Catalog.GetString ("LaborTypeHolidayTime");
			default:
				return type.ToString ();
			}
		}*/

		/// <summary>
		/// Helper method to safely convert a string to a double
		/// </summary>
		public static double ToDouble (this string text, IFormatProvider provider)
		{
			double x;
			double.TryParse (text, NumberStyles.Any, provider, out x);
			return x;
		}

		/// <summary>
		/// Helper method to safely convert a string to a decimal
		/// </summary>
		public static decimal ToDecimal (this string text, IFormatProvider provider)
		{
			decimal x;
			decimal.TryParse (text, NumberStyles.Any, provider, out x);
			return x;
		}

		/// <summary>
		/// Helper method to safely convert a string to a int
		/// </summary>
		public static int ToInt (this string text, IFormatProvider provider)
		{
			int value = 0;
			int.TryParse (text, NumberStyles.Any, provider, out value);
			return value;
		}

		#if NETFX_CORE
		/// <summary>
		/// Missing functionality for WinRT
		/// </summary>
		public static string ToShortDateString (this DateTime date)
		{
		return date.ToString (DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
		}

		/// <summary>
		/// Missing functionality for WinRT
		/// </summary>
		public static string ToShortTimeString (this DateTime date)
		{
		return date.ToString (DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
		}
		#endif
	}
}

