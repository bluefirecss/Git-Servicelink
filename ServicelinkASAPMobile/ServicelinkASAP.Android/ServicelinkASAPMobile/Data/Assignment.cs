using System;
using System.Collections.Generic;

using ServicelinkASAPMobile;
using ServicelinkASAPMobile.Data;
using ServicelinkASAPMobile.Service;

namespace ServicelinkASAP
{
	public class Assignment
	{
		public AssignmentType Type { get; set; }

		public Posting posting { get; set; }

		public Sale sale { get; set; }

	}


}

