
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicelinkASAPMobile.Data {
	/// <summary>
	/// An enumeration for Assignment status
	/// </summary>
	public enum PostingStatus {
		New = 0,
		Hold = 1,	
		Uploaded = 2,	
		Complete = 3,
		Rejected = 4,
		Exception = 9999
	}

	/// <summary>
	/// An enumeration for Labor types
	/// </summary>
	public enum DocumentType {
		NOS = 0,
		APPT = 1,
		Notary = 2,
		Agreement = 3
		//Others = 4
	}

	public enum Extensoin {
		TIFF = 0,
		PDF = 1,
		PNG = 2

	}


}