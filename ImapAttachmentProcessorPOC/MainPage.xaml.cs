using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ImapAttachmentProcessorPOC.Shared;
using ImapAttachmentProcessorPOC.Models;
using S22.Imap;
using Newtonsoft.Json;
using System.Net.Mail;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImapAttachmentProcessorPOC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			//ResultTextBlock.Text = "Button Clicked!";
			ResultTextBlock.Text = StartDate.Date.ToString();
			downloadAndCleanAttachments(StartDate.Date, EndDate.Date);
		}

		private void ResultTextBlock_SelectionChanged(object sender, RoutedEventArgs e)
		{

		}

		private void ClickMeButton_Click(object sender, RoutedEventArgs e)
		{
			ResultTextBlock.Text = "What is XAML?";
		}

		private void PageLoaded(object sender, RoutedEventArgs e)
		{
			Button myButton = new Button();
			myButton.Name = "ClickMeButton";
			myButton.Content = "Click Me";
			myButton.Width = 200;
			myButton.Height = 100;
			myButton.Margin = new Thickness(20, 20, 0, 0);
			myButton.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
			myButton.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;

			myButton.Background = new SolidColorBrush(Windows.UI.Colors.Red);
			myButton.Click += ExportButton_Click;

			LayoutGrid.Children.Add(myButton);


		}

		private async void downloadAndCleanAttachments(DateTimeOffset s, DateTimeOffset e)
		{

			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;

			String hostname = "imap.gmail.com";
			String username = "xxxxxxxx@gmail.com";
			String password = "xxxxxxx";

			using (ImapClient Client = new ImapClient(hostname, 993, username, password, AuthMethod.Login, true))
			{
				Console.WriteLine("We are connected!");

				IEnumerable<uint> uids = Client.Search(
					SearchCondition.SentSince(new DateTime(2019, 7, 16))
					.And(SearchCondition.SentBefore(e.DateTime))
				);
				Console.WriteLine("===========================================================");
				Console.WriteLine(uids);
				Console.WriteLine("===========================================================");

				// The expression will be evaluated for every MIME part
				// of every mail message in the uids collection.
				IEnumerable<MailMessage> messages = Client.GetMessages(uids,
					(Bodypart part) => {
						// We're only interested in attachments.
						if (part.Disposition.Type == ContentDispositionType.Attachment)
						{
							Int64 TwoMegabytes = (1024 * 1024 * 2);
							if (part.Size > TwoMegabytes)
							{
								// Don't download this attachment
								return false;
							}
						}

						// Fetch MIME part and include it in the returned MailMessage instance.
						return true;
					}
				);

				Console.WriteLine("messages retrieved:");
				Console.WriteLine("-------------------------------------------------------------");
				Console.WriteLine(messages);
				Console.WriteLine("-------------------------------------------------------------");

				List<Models.RawTextFile> cleanedSensorDataFiles = new List<Models.RawTextFile>();

				int i = 0;
				foreach (var msg in messages)
				{
					i++;
					foreach (var attachment in msg.Attachments)
					{
						Console.WriteLine("processing attachment : " + attachment.Name);

						if (attachment.Name == "report.json")
						{
							StreamReader reader = new StreamReader(attachment.ContentStream);
							ReportData reportData = JsonConvert.DeserializeObject<ReportData>(reader.ReadToEnd());
							Console.WriteLine("-------------------------------------------------------------");
							Console.WriteLine(reportData.location.latitude);
							Console.WriteLine("-------------------------------------------------------------");
						}
						else if (attachment.Name == "130500040340_201906271342.txt")
						{
							Console.WriteLine("-------------------------------------------------------------");
							StreamReader reader = new StreamReader(attachment.ContentStream);
							string line = "";
							//int counter = 0;
							int lastLevel = 0;
							//String child = "";
							String parent = "";
							//String path = "";

							List<String> path = new List<String>();
							//path.Add("Root");

							//string path = "";
							RawTextFile rawTextFile = new RawTextFile();
							String field = "";
							String value = "";
							String fullFieldPath = "";

							Dictionary<string, string> mappedFields = new Dictionary<string, string>();
							//mappedFields.Add("Root.Device", "Device");
							//mappedFields.Add("Root.Vers", "Vers");

							while ((line = reader.ReadLine()) != null)
							{
								// 00 - print line
								//Console.WriteLine("line: " + line);

								// 01 - split line into fields
								string[] words = line.Split(':');
								Console.WriteLine("------------------");
								Console.WriteLine(line);
								Console.WriteLine("------------------");


								int currLevel = words[0].TakeWhile(Char.IsWhiteSpace).Count();
								Console.WriteLine("currLevel: " + currLevel);

								//applyNamingConventionToPropertyName;

								// 01 - get string array element values from line
								field = Helpers.getStringArrayElementValue(words, 0);
								words[0] = field;
								value = Helpers.getStringArrayElementValue(words, 1);

								Console.WriteLine("field: " + field);
								Console.WriteLine("value: " + value);

								// 01 - get current parent name
								parent = Helpers.getCurrentParent(words, parent);
								Console.WriteLine("parent: " + parent);



								// 02 - get indentation level


								// 03 - get path using indentation level
								path = Helpers.getCurrentPath(path, words, parent, currLevel, lastLevel);
								//Console.WriteLine("path: " + path);

								// 03 - get full field name
								fullFieldPath = Helpers.getFullFieldName(path, field, value);
								Console.WriteLine("fullFieldPath: " + fullFieldPath);


								//string value = someData.GetType().GetProperty(key).GetValue(someData).ToString();
								//rawTextFile.GetType().GetProperty("Conf.Lot").SetValue(rawTextFile, value);
								//rawTextFile.GetType().GetProperty("Conf").GetType().GetProperty("Lot").SetValue(rawTextFile, value);
								//rawTextFile.GetType().GetProperty("Conf").GetType().GetProperty("Lot").SetValue(rawTextFile, value);

								//var address = GetPropertyValue(GetPropertyValue(rawTextFile, "Conf"), "Lot");

								//address = "testing";
								//SetProperty(string compoundProperty, object target, object value)

								if (parent == "Data" || fullFieldPath.Contains("Conf.Alarm.Zero.TAL") || fullFieldPath.Contains("Conf.Alarm.One.TAL") || fullFieldPath.Contains("Conf.ExtSensor") || fullFieldPath.Contains("Conf.IntSensor") || fullFieldPath.Contains("Conf.TestRes"))
								{
									Helpers.SetProperty(line, rawTextFile, value, parent, fullFieldPath);
								}
								else
								{
									Helpers.SetProperty(fullFieldPath, rawTextFile, value, parent, fullFieldPath);
								}



								// 04 - record indendentation level
								lastLevel = currLevel;

								// 05 - print current path
								Helpers.printCurrentPathToConsole(path, field, value);

								//counter++;
							} //end read lines
							string json = JsonConvert.SerializeObject(rawTextFile);
							cleanedSensorDataFiles.Add(rawTextFile);
							//System.IO.File.WriteAllText(@"c:\_tmp\test_text_file.json", json);
							String localFileName = tempFolder.Path + @"\test_text_file.json";
							System.IO.File.WriteAllText(localFileName, json);
							ResultTextBlock.Text = localFileName;
							Console.WriteLine("Line96");
						} // end processing raw text file
					} //end processing attachments
				} //end processing messages
			} //end imap client

		}
	}
}
