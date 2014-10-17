using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicelinkASAPMobile.Utilities
{
	public class ErrorHandling
	{

		private const string LoginError = "Invalid UserName/Password.";
		private const string LoginValidation = "UserName and Password cannot be empty";
		private const string RequiredField = "Required field: ";
		private const string SystemException = "System error, message: ";
		private const string ErrorMessage = "Error: ";
		private const string SucceedMessage = "Action succeeded";
		private const string FailedMessage = "Action failed";

		public static string GetErrorString (string key, string message = null)
		{
			switch (key) {
			case "LoginError":
				return LoginError;
			case "LoginValidation":
				return LoginValidation;
			case "RequiredField":
				return RequiredField + message;
			case "ErrorMessage":
				return ErrorMessage + message;
			case "SystemException":
				return SystemException + message;
			case "SucceedMessage":
				return SucceedMessage + (message == null ? "." : ": " + message + ".");
			case "FailedMessage":
				return FailedMessage + (message == null ? "." : " due to " + message + ".");
			default:
				return string.Empty;
			}
		}
	}
}

