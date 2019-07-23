using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImapAttachmentProcessorPOC.Shared
{
	class Helpers
	{
		// Traverse path structure using relative path indentation change and property name/value combinations
		public static List<String> getCurrentPath(List<String> path, String[] words, String parent, int currLevel, int lastLevel)
		{
			// go down a level if starting point is root path AND indentation has changed (e.g., root level to "Conf")
			if (words[1] == "" && (currLevel > lastLevel))
			{
				path.Add(parent); 
			// go up a level if indentation has changed
			} else if (currLevel < lastLevel) {
				// make a lateral level move if going to a property **name only** line (e.g., "Conf.Alarm.Zero" to "Conf.Alarm.One")
				if (words[1] == "") // (e.g., "words[0] = 'Conf' and words[1] = '')
				{
					// capture relative level change
					int j = lastLevel - currLevel;
					// step 01 - go up a level (e.g., "Conf.Alarm.Zero" to "Conf.Alarm")
					// root level represented by an empty list - code prevents IndexOutOfRangeException
					if (path.Any()) 
					{
						// go up two levels (TODO - refactor to handle 3+ level jumpbs - not currently a requirement)
						if (j == 2)
						{
							path.RemoveAt(path.Count - 1);
							path.RemoveAt(path.Count - 1);
						}
						// othwerwise go up only a single level
						else
						{
							path.RemoveAt(path.Count - 1);
						}

					}
					// step 02 - go down a level (e.g., "Conf.Alarm" to "Conf.Alarm.One")
					path.Add(parent);
				// go up a level if moving up to property **name and value** line (e.g., "Cert.Public Key: 804343.." to "Sig Cert:0bd6"
				} else { 
					if (path.Any()) //prevent IndexOutOfRangeException for empty list
					{
						path.RemoveAt(path.Count - 1);
					}
				}
			// go down a level if current line is a property **name only** line and if indentation has not changed
			// e.g., go from root level property name and value line (e.g., "Ext Sensor: 1" to "Conf:"
			} else if (words[1] == "" && (currLevel == lastLevel)) // it is possible to move down a level even if indentation has not changed
			{
				path.Add(parent);
			}

			return path;

		}

		public static void printCurrentPathToConsole(List<String> p, String f, String v)
		{

			String path = "";
			// an empty list represents the root path - an empty path string represents the root level
			if (p.Count == 0) {
				path = "";
			// no string concatentation required if just a single path component
			} else if (p.Count == 1) {
				path = p[0];
			} else {
				path = p.Aggregate((i, j) => i + "." + j.Trim());
			}
			path = $"{path} --> {f.Trim()} --> {v.Trim()}";
		}

		public static String getCurrentParent(String[] words, String lastParent)
		{

			if (words[1] == "")
			{
				return words[0];
			}
			else
			{
				return lastParent;
			}

		}

		static void SaveAttachmentToDisk(System.Net.Mail.Attachment attachment)
		{

			using (var fileStream = File.Create("C:\\_tmp\\imap\\" + attachment.Name))
			{
				attachment.ContentStream.Seek(0, SeekOrigin.Begin);
				attachment.ContentStream.CopyTo(fileStream);
			}

		}

		public static String getStringArrayElementValue(string[] array, int index)
		{
			String s = "";
			if (array.Length > index) // prevents System.IndexOutOfRangeException
			{
				s = array[index];
				if (index == 0)
				{
					s = Regex.Replace(s, "[0]", "Zero");
					s = Regex.Replace(s, "[1]", "One");
				}
				if (index == 0)
				{
					//s = s.Trim();
					s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());

					//Handle acronyms
					String cleaned = "";
					var words = s.Split(" ");
					foreach (var word in words)
					{
						String w = "";
						if (word.Length > 0 && word.Length < 3)
						{
							w = word.ToUpper();
						}
						else
						{
							w = word;
						}
						cleaned = cleaned + w;
					}

					s = cleaned.Replace(" ", String.Empty);

				}
				return s;
			}
			else
			{
				return "";
			}
		}

		public static String getFullFieldName(List<String> p, String f, String v)
		{

			String path = "";

			if (p.Count == 0)
			{
				path = "";
			}
			else
			{
				path = p.Aggregate((i, j) => i + "." + j.Trim());
			}


			f = f.Replace(" ", String.Empty);
			f = Regex.Replace(f, "[0]", "Zero");
			f = Regex.Replace(f, "[1]", "One");

			if (v != "" && path != "")
			{
				path = $"{path}.{f.Trim()}";
			}
			else if (p.Count == 0)
			{
				path = $"{f.Trim()}";
			}


			return path;

		}

		public static object GetPropertyValue(object obj, string propertyName)
		{
			var objType = obj.GetType();
			PropertyInfo prop = objType.GetProperty(propertyName);
			return prop.GetValue(obj, null);
		}

		public static void SetProperty(string compoundProperty, object target, String value, String parent, String fullFieldPath)
		{

			if (value != "")
			{
				string[] bits = compoundProperty.Split('.');

				if (parent == "Data")
				{
					String[] dataCols = null;
					dataCols = compoundProperty.Split('\t');

					//ignore the header row
					if (dataCols[1] != "yyyy-MM-dd hh:mm")
					{

						PropertyInfo parentProperty = target.GetType().GetProperty("Data");
						List<Models.SensorData> childDataObject = (List<Models.SensorData>)parentProperty.GetValue(target, null);

						Models.SensorData sd = new Models.SensorData();

						for (int i = 0; i <= dataCols.Length - 1; i++)
						{
							if (i == 1)
							{
								//PropertyInfo propertyToSet = childDataObject.GetType().GetProperty("DateTime");
								//propertyToSet.SetValue(childDataObject, dataCols[i], null);
								sd.DateTime = dataCols[i];
							}
							else if (i == 2)
							{
								//PropertyInfo propertyToSet = childDataObject.GetType().GetProperty("T");
								//propertyToSet.SetValue(childDataObject, dataCols[i], null);
								sd.T = dataCols[i];
							}
							else if (i == 3)
							{
								//PropertyInfo propertyToSet = childDataObject.GetType().GetProperty("OutOfLimits");
								//propertyToSet.SetValue(childDataObject, dataCols[i], null);
								sd.OutOfLimits = dataCols[i];
							}
						}

						childDataObject.Add(sd);
						//target.Data.DateTime = dataCols[0];

						//PropertyInfo childProperty = parentProperty.GetType().GetProperty("DateTime");

						Console.WriteLine("line128");

					}


					//|| fullFieldPath.Contains("Conf.ExtSensor")
				}
				else if (fullFieldPath.Contains("Conf.Alarm.Zero.TAL") || fullFieldPath.Contains("Conf.Alarm.One.TAL") || fullFieldPath.Contains("Conf.ExtSensor") || fullFieldPath.Contains("Conf.IntSensor") || fullFieldPath.Contains("Conf.TestRes"))
				{

					bits = fullFieldPath.Split('.');
					String[] rows = compoundProperty.Split(',');
					for (int i = 0; i < rows.Length; i++)
					{
						object targetTmp = target;
						for (int j = 0; j < bits.Length - 1; j++)
						{
							PropertyInfo propertyToGet = targetTmp.GetType().GetProperty(bits[j]);
							targetTmp = propertyToGet.GetValue(targetTmp, null);
						}

						String[] data = rows[i].Split(':');

						PropertyInfo propertyToSet = null;

						if (rows[i].Contains("T AL"))
						{
							propertyToSet = targetTmp.GetType().GetProperty("UpperTAL");
						}
						else if (rows[i].Contains("t AL"))
						{
							propertyToSet = targetTmp.GetType().GetProperty("LowerTAL");
						}
						else if (rows[i].Contains("Timeout"))
						{
							propertyToSet = targetTmp.GetType().GetProperty("Timeout");
						}
						else if (rows[i].Contains("Offset"))
						{
							propertyToSet = targetTmp.GetType().GetProperty("Offset");
						}
						else if (rows[i].Contains("Test Res"))
						{
							propertyToSet = targetTmp.GetType().GetProperty("TestRes");
						}
						else if (rows[i].Contains("Test TS"))
						{
							propertyToSet = targetTmp.GetType().GetProperty("TestTS");
						}

						if (propertyToSet != null)
						{
							if (rows[i].Contains("Test TS"))
							{
								propertyToSet.SetValue(targetTmp, data[1] + ":" + data[2], null);
							}
							else
							{
								propertyToSet.SetValue(targetTmp, data[1], null);
							}

						}




					}
					Console.WriteLine("line418");


				}
				else
				{
					String propertyName = "";
					Console.WriteLine("compoundProperty: " + compoundProperty);

					if (compoundProperty.Contains("Conf.Alarm.Zero.TAL"))
					{
						String[] vals = value.Split(',');
						Console.WriteLine("line417");
					}

					for (int i = 0; i < bits.Length - 1; i++)
					{
						if (bits[i] == "Alarm")
						{
							Console.WriteLine("line325");
						}

						//propertyName = bits[i].Replace(" ", String.Empty);
						//propertyName = applyNamingConventionToPropertyName(bits[i]);
						PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
						target = propertyToGet.GetValue(target, null);

					}
					//propertyName = bits.Last().Replace(" ", String.Empty);
					//propertyName = applyNamingConventionToPropertyName(bits.Last());
					PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
					if (propertyToSet != null)
					{
						propertyToSet.SetValue(target, value, null);
					}
				}



			}

		}

		public static String applyNamingConventionToPropertyName(String s)
		{
			//s = Regex.Replace(s, "[0]", "Zero");
			//s = Regex.Replace(s, "[1]", "One");
			//s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
			String cleaned = "";
			var words = s.Split(" ");
			/*
			foreach (var word in words)
			{
				String w = "";
				if (word.Length > 0 && word.Length < 3)
				{
					w = word.ToUpper();
				} else
				{
					w = word;
				}
				cleaned = cleaned + w;
			}
			*/

			//s = s.Replace(" ", String.Empty);
			return cleaned;
		}







	}
}
