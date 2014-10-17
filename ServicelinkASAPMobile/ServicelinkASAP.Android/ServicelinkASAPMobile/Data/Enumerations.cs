
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
		Cancelled = 5,
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

	public enum AssignmentType{
		Postings = 0,
		Sales = 1
	}

	public enum PostingLocation{
		FrontDoor = 0,
		Gate = 1,
		Other =2
	}

	public enum Occupancy{
		Occupied =0,
		Unoccupied =1,
		Unknown =2,
		VacantLand = 3
	}

	public enum PropertyType{
		Residential = 0,
		Commercial = 1,
		Rural =2
	}
}