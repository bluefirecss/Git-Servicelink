using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace ServicelinkASAPMobile.Data
{
	public class Signature {

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Indexed]
		public int DocumentId { get; set; }

		public int OrderID { get; set; }

		public byte [] Image { get; set; }
	}
}

