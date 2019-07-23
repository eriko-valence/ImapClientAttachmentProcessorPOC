using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapAttachmentProcessorPOC.Models
{
	public class ReportData
	{
		public AppInfo appInfo { get; set; }
		public Facility facility { get; set; }
		public Location location { get; set; }
		public Refrigerator refrigerator { get; set; }
		public String timestamp { get; set; }

	}

	public class AppInfo
	{
		public String os { get; set; }
		public String osVersion { get; set; }
		public String phoneModel { get; set; }
		public String version { get; set; }

	}

	public class Facility
	{
		public String country { get; set; }
		public String district { get; set; }
		public String managerName { get; set; }
		public String managerPhone { get; set; }
		public String name { get; set; }
		public String state { get; set; }

	}

	public class Location
	{
		public String accuracy { get; set; }
		public String latitude { get; set; }
		public String longitude { get; set; }

	}

	public class Refrigerator
	{
		public String deviceId { get; set; }
		public String manufacturer { get; set; }
		public String model { get; set; }

	}
}
