using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KV4S.AR.DMR.BM.TG.Exporter
{
    class Program
    {
        public static string URL = "https://api.brandmeister.network/v1.0/groups/";
        //public static string AnyTonecsvFile = Environment.CurrentDirectory + @"\AnyTone_TGs.csv";
        public static string AnyTonecsvFile = Environment.CurrentDirectory + @"\" + ConfigurationManager.AppSettings["ExportFileName"];
        public static string ExtrasFile = Environment.CurrentDirectory + @"\" + ConfigurationManager.AppSettings["ExtraTGList"];

        private static List<string> _startsWithList = null;
        private static string StartsWithList
        {
            set
            {
                string[] startsWithArray = value.Split(',');
                _startsWithList = new List<string>(startsWithArray.Length);
                _startsWithList.AddRange(startsWithArray);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                int i = 1; //column A iterator for file spec.

                //delete files before begin
                File.Delete(AnyTonecsvFile);

                Console.WriteLine("Welcome to the TG Flat file generator Application by KV4S!");
                Console.WriteLine(" ");
                Console.WriteLine("Beginning download from " + URL);
                Console.WriteLine("Please Stand by.....");
                Console.WriteLine(" ");

                var client = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var stream = client.OpenRead(URL))
                using (var reader = new StreamReader(stream))
                {
                    bool lineAdd = false; //initialize to false.
                    string line; //varaible retained for use outside of loop.
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!line.Contains('{') && !line.Contains('}')) //get rid of un-needed json syntax
                        {
                            string[] strSplit = line.Split('"');

                            //Data Filtering (flags a line to save or not)
                            if (Convert.ToInt64(strSplit[1]) <= 95) //keep these Global TGs reguardless of filter.
                            {
                                if (ConfigurationManager.AppSettings["LessThan91"] == "N")
                                {
                                    if (Convert.ToInt64(strSplit[1]) >= 91)
                                    {
                                        lineAdd = true;
                                    }
                                }
                                else
                                {
                                    lineAdd = true;
                                }
                            }
                            else
                            {
                                StartsWithList = ConfigurationManager.AppSettings["IDStartsWith"]; //if filter is blank gets everything.
                                foreach (string value in _startsWithList)
                                {
                                    if (strSplit[1].StartsWith(value))
                                    {
                                        lineAdd = true;
                                    }
                                    if (strSplit[3].ToUpper().Contains("EMCOM")) //include emcom requardless of filter.
                                    {
                                        lineAdd = true;
                                    }
                                }
                            }

                            //Save data to file
                            if (ConfigurationManager.AppSettings["AnyTone"] == "Y") //file spec for AnyTone (so far only radio that can import a TG list)
                            {
                                if (ConfigurationManager.AppSettings["IDinsteadOfName"] == "Y") //feature request to display the TG ID instead of TG name.
                                {
                                    if (lineAdd)
                                    {
                                        SaveAnyToneCSV(i, strSplit[1], strSplit[1], "Group Call", "None");
                                        i++;
                                    }
                                }
                                else
                                {
                                    if (lineAdd)
                                    {
                                        SaveAnyToneCSV(i, strSplit[1], strSplit[3], "Group Call", "None");
                                        i++;
                                    }
                                }
                            }
                            lineAdd = false;
                        }
                    }
                }

                //Load Extra Talk Groupds from file
                if (File.Exists(ExtrasFile))
                {
                    Console.WriteLine("Loading extras.");
                    using (StreamReader sr = File.OpenText(ExtrasFile))
                    {
                        String s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (!s.Contains("No.,Radio ID,Name,Call Type,Call Alert"))
                            {
                                string[] strSplit = s.Split(',');
                                SaveAnyToneCSV(i, strSplit[1], strSplit[2], strSplit[3], strSplit[4]);
                                i++;
                            }
                        }
                    }
                }

                if (ConfigurationManager.AppSettings["AnyTone"] == "Y") //potential for different file names per radio.
                {
                    Console.WriteLine("Export created at: " + AnyTonecsvFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program encountered and error:");
                Console.WriteLine(ex.Message);
                LogError(ex.Message, ex.Source);
            }
            finally
            {
                if (ConfigurationManager.AppSettings["Unattended"] == "N")
                {
                    Console.WriteLine("Press any key on your keyboard to quit...");
                    Console.ReadKey();
                }
            }
        }

        public static void SaveAnyToneCSV(int intLoopID, string tgID, string tgDesc, string callType, string callAlert)
        {
            tgDesc = tgDesc.Replace(" NO NETS!!!", "");
            tgDesc = tgDesc.Replace("Tac 310 NOT A CALL CHANNEL", "TAC 310 USA");
            tgDesc = tgDesc.Replace(" - 10 Minute Limit", "");
            FileInfo fi = new FileInfo(AnyTonecsvFile);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(AnyTonecsvFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            //write header
            if (intLoopID == 1)
            {
                //textBox1.Text = "She said, \"You deserve a treat!\" ";
                sw.WriteLine("\"No.\",\"Radio ID\",\"Name\",\"Call Type\",\"Call Alert\"");
            }

            if (tgID == "9990") //needs to be a private call for Parrot not group.
            {
                sw.WriteLine("\"" + intLoopID + "\",\"" + tgID + "\",\"" + tgDesc + "\",\"Private Call\",\"None\"");
                Console.WriteLine("\"" + intLoopID + "\",\"" + tgID + "\",\"" + tgDesc + "\",\"Private Call\",\"None\"");
            }
            else
            {
                sw.WriteLine("\"" + intLoopID + "\",\"" + tgID + "\",\"" + tgDesc + "\",\"" + callType + "\",\"" + callAlert + "\"");
                Console.WriteLine("\"" + intLoopID + "\",\"" + tgID + "\",\"" + tgDesc + "\",\"" + callType + "\"" + callAlert + "\"");
            }
            sw.Close();
            fs.Close();
        }

        private static void LogError(string Message, string source)
        {
            try
            {
                FileStream fs = null;
                fs = new FileStream("ErrorLog.txt", FileMode.Append);
                StreamWriter log = new StreamWriter(fs);
                log.WriteLine(DateTime.Now + " Error: " + Message + " Source: " + source);
                log.Close();
                fs.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error logging previous error.");
                Console.WriteLine("Make sure the Error log is not open.");
            }
        }
    }
}
