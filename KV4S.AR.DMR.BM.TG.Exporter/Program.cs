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
        public static string AnyTonecsvFile = Environment.CurrentDirectory + @"\AnyTone_TGs.csv";

        static void Main(string[] args)
        {
            try
            {
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
                    int i = 1;
                    bool lineAdd = false;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!line.Contains('{') && !line.Contains('}'))
                        {
                            string[] strSplit = line.Split('"');

                            //Data Filtering
                            if (ConfigurationManager.AppSettings["US"] == "Y")
                            {
                                if (Convert.ToInt16(strSplit[1]) <= 9 || Convert.ToInt16(strSplit[1]) == 91 || Convert.ToInt16(strSplit[1]) == 93 || Convert.ToInt16(strSplit[1]) == 9990)
                                {
                                    lineAdd = true;
                                }
                                else if (strSplit[1].StartsWith("31"))
                                {
                                    lineAdd = true;
                                }
                                else
                                {
                                    lineAdd = false;
                                }
                            }
                            else if (ConfigurationManager.AppSettings["UK"] == "Y")
                            {
                                if (Convert.ToInt16(strSplit[1]) <= 9 || Convert.ToInt16(strSplit[1]) == 91 || Convert.ToInt16(strSplit[1]) == 92 || Convert.ToInt16(strSplit[1]) == 9990)
                                {
                                    lineAdd = true;
                                }
                                else if (strSplit[1].StartsWith("235"))
                                {
                                    lineAdd = true;
                                }
                                else
                                {
                                    lineAdd = false;
                                }
                            }
                            else
                            {
                                lineAdd = true;
                            }


                            //Save data to file
                            if (ConfigurationManager.AppSettings["AnyTone"] == "Y")
                            {
                                if (ConfigurationManager.AppSettings["IDinsteadOfName"] == "Y")
                                {
                                    if (lineAdd)
                                    {
                                        SaveAnyToneCSV(i, strSplit[1], strSplit[1]);
                                    }
                                }
                                else
                                {
                                    if (lineAdd)
                                    {
                                        SaveAnyToneCSV(i, strSplit[1], strSplit[3]);
                                    }
                                }
                            }
                            lineAdd = false;
                            i++;
                        }
                    }
                    if (ConfigurationManager.AppSettings["AnyTone"] == "Y")
                    {
                        Console.WriteLine("Export created at: " + AnyTonecsvFile);
                    }
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

        public static void SaveAnyToneCSV(int intLoopID, string tgID, string tgDesc)
        {
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

            sw.WriteLine("\"" + intLoopID + "\",\"" + tgID + "\",\"" + tgDesc + "\",\"Group Call\",\"None\"");
            Console.WriteLine("\"" + intLoopID + "\",\"" + tgID + "\",\"" + tgDesc + "\",\"Group Call\",\"None\"");
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
