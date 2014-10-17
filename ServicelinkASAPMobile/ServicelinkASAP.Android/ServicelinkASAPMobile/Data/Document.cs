
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;

namespace ServicelinkASAPMobile.Data {

	public class Document {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public int OrderID { get; set; }

		public string Title { get; set; }

		/// <summary>
		/// File path to the document
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// The document type, this determines the color for the UI on some platforms
		/// </summary>
		public DocumentType Type { get; set; }

		public Extensoin Extension { get; set; }
	}
}

