using System;
using System.IO;   
using System.Net;
using System.Xml;
using System.Text;

// https://opengeo.ncep.noaa.gov/geoserver/conus/conus_cref_qcd/ows?service=wms&version=1.3.0&request=GetCapabilities

namespace GetCurrentWeatherImage
{
    class Program
    {
        static bool GetURL(String url, String filename)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Console.WriteLine("Requesting:" + url);

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36";

            try
            {
                WebResponse response = myRequest.GetResponse();

                Console.WriteLine("Retrieveing...");

                Stream input = response.GetResponseStream();

                Stream output = File.OpenWrite(filename);

                input.CopyTo(output);

                input.Close();

                output.Close();

                Console.WriteLine("Completed");

                return true;
            }
            catch (System.Net.WebException we)
            {
                Console.WriteLine(we.Message);

                return false;
            }

        }

        static void Main(string[] args)
        {
            String time = "";

            if (GetURL("https://opengeo.ncep.noaa.gov/geoserver/conus/conus_cref_qcd/ows?service=wms&version=1.3.0&request=GetCapabilities", "caps.xml"))
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.DtdProcessing = DtdProcessing.Parse;

                XmlReader reader = XmlReader.Create("caps.xml", settings);

                reader.MoveToContent();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                        {
                            switch (reader.Name)
                            {
                                case "Dimension":
                                {
                                    String[] times = reader.ReadInnerXml().Split(',');

                                    for (Int32 i = times.Length - 1;i > -1; i--)
                                    {
                                        time = times[i].Replace("-", "").Replace("T", "_").Replace(":", "").Replace(".000Z", "");

                                        if (GetURL("https://mrms.ncep.noaa.gov/data/RIDGEII/L2/CONUS/CREF_QCD/CONUS_L2_CREF_QCD_" + time + ".tif.gz", "weather.tif.gz"))
                                        {
                                            Stream output = File.OpenWrite("weather.tif.bat");

                                            String buffer = "\"C:\\Program Files\\7-Zip\\7z.exe\" e \"weather.tif.gz\" \"*.*\"\r\n";

                                            buffer += "\r\n";

                                            buffer += "REN \"CONUS_L2_CREF_QCD_" + time + ".tif\" \"weather.tif\"\r\n";

                                            output.Write(Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);

                                            output.Close();

                                            break;
                                        }

                                    }

                                    break;
                                }
                            }

                            break;
                        }
                    }
                }

                reader.Close();
            }

        }
    }
}
