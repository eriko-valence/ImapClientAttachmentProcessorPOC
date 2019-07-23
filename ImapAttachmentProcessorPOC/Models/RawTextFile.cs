using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapAttachmentProcessorPOC.Models
{
	public class RawTextFile
	{
		public RawTextFile()
		{
			this.Conf = new Conf();
			this.Cert = new Cert();
			this.Hist = new Hist();
			this.Data = new List<SensorData>();

			this.Conf.Alarm = new ConfigAlarm();
			this.Conf.Alarm.Zero = new ConfigAlarmEntry();
			this.Conf.Alarm.One = new ConfigAlarmEntry();
			this.Conf.ExtSensor = new ConfigSensor();
			this.Conf.IntSensor = new ConfigSensor();

			this.Hist.One = new HistOne();
			this.Hist.One.Alarm = new HistAlarm();
			this.Hist.One.Alarm.Zero = new HistAlarmEntry();
			this.Hist.One.Alarm.One = new HistAlarmEntry();
			this.Hist.One.ExtSensorTimeout = new HistSensorTimeout();
			this.Hist.One.IntSensorTimeout = new HistSensorTimeout();

		}

		public String Device { get; set; }
		public String Vers { get; set; }
		public String FWVers { get; set; }
		public String Sensor { get; set; }
		public String ExtSensor { get; set; }
		public Conf Conf { get; set; }
		public List<SensorData> Data { get; set; }
		public Cert Cert { get; set; }
		public Hist Hist { get; set; }
		public String SigCert { get; set; }
		public String Sig { get; set; }

	}

	public class Conf
	{
		public String Serial { get; set; }
		public String Pcb { get; set; }
		public String Cid { get; set; }
		public String Lot { get; set; }
		public String Zone { get; set; }
		public String MeasurementDelay { get; set; }
		public String MovingAvrg { get; set; }
		public String UserAlarmConfig { get; set; }
		public String AlarmIndication { get; set; }
		public String TempUnit { get; set; }
		public ConfigAlarm Alarm { get; set; }
		public ConfigSensor ExtSensor { get; set; }
		public ConfigSensor IntSensor { get; set; }
		public String ReportHistoryLength { get; set; }
		public String DetReport { get; set; }
		public String UseExtDevices { get; set; }
		public String LoggingInterval { get; set; }
		public String TestRes { get; set; }
		public String TestTS { get; set; }


	}

	public class ConfigAlarm
	{
		public ConfigAlarmEntry Zero { get; set; }
		public ConfigAlarmEntry One { get; set; }

	}

	public class ConfigAlarmEntry
	{
		public String UpperTAL { get; set; }
		public String LowerTAL { get; set; }

	}

	public class ConfigSensor
	{
		public String Timeout { get; set; }
		public String Offset { get; set; }
	}

	public class Hist
	{
		public String TSActv { get; set; }
		public String TSReportCreation { get; set; }
		public HistOne One { get; set; }

	}

	public class HistOne
	{
		public String Date { get; set; }
		public String MinT { get; set; }
		public String MaxT { get; set; }
		public String AvrgT { get; set; }
		public HistAlarm Alarm { get; set; }

		public HistSensorTimeout ExtSensorTimeout { get; set; }
		public HistSensorTimeout IntSensorTimeout { get; set; }
		public String Events { get; set; }
	}

	public class HistAlarm
	{
		public HistAlarmEntry Zero { get; set; }
		public HistAlarmEntry One { get; set; }
	}

	public class HistAlarmEntry
	{
		public String TAcc { get; set; }
	}

	public class HistSensorTimeout
	{
		public String TAccst { get; set; }
	}

	public class SensorData
	{
		public String DateTime { get; set; }
		public String T { get; set; }
		public String OutOfLimits { get; set; }
	}

	public class Cert
	{
		public String Vers { get; set; }
		public String Lot { get; set; }
		public String Issuer { get; set; }
		public String ValidFrom { get; set; }
		public String Owner { get; set; }
		public String PublicKey { get; set; }
	}
}
