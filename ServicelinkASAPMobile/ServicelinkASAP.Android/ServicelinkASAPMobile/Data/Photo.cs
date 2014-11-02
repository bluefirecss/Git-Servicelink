using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicelinkASAPMobile.Utilities;

namespace ServicelinkASAPMobile.Data
{
    [Serializable()]
    public class Photo
    {
        [PrimaryKey, AutoIncrement]
		public int Id { get; set; }

        public string OrderID { get; set; }
        public string FileName { get; set; }
        public string DateCreated { get; set; }
        public double Latitude { get; set; }
		public double Longitude { get; set; }

    }
}
