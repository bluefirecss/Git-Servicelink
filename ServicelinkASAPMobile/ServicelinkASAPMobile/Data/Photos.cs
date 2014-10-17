using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServicelinkASAPMobile.Utilities;

namespace ServicelinkASAPMobile.Data
{
    [Serializable()]
    public class Photos
    {
        [PrimaryKey, AutoIncrement]
		public int Id { get; set; }

        public string OrderID { get; set; }
        public string FileName { get; set; }
        public string DateCreated { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

    }
}
