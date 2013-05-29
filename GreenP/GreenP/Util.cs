using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Bing.Maps;

namespace GreenP
{
    class Util
    {
        public static Dictionary<Pushpin,JsonObject> doInit()
        {
            String jsonSource = getData();
            Debug.WriteLine(jsonSource);

            JsonObject carparks = JsonObject.Parse(jsonSource);
            
            Dictionary<Pushpin,JsonObject> ret = new Dictionary<Pushpin, JsonObject>();

            foreach (JsonValue carpark in carparks.GetNamedArray("carparks"))
            {
                ret.Add(new Pushpin(),carpark.GetObject());
            }

            return ret;

        }

        //get up to date json
        public static string getData()
        {
            HttpWebRequest g = WebRequest.CreateHttp("http://www1.toronto.ca/City_Of_Toronto/Information_&_Technology/Open_Data/Data_Sets/Assets/Files/greenPParking.json");
            g.Proxy = null;
            Task<WebResponse> task = g.GetResponseAsync();
            //task.Start();
            while (!task.IsCompleted);
            WebResponse response = task.Result;
            Stream receiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            string ret = "";
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, encode);
            Char[] read = new Char[256];
            // Reads 256 characters at a time.     
            //int count = readStream.Read(read, 0, 256);
            //while (count > 0)
            //{
                // Dumps the 256 characters on a string and displays the string to the console.
                ret += readStream.ReadToEnd();
            //    count = readStream.Read(read, 0, 256);
            //}
            // Releases the resources of the response.
            response.Dispose();
            // Releases the resources of the Stream.
            readStream.Dispose();
            return ret;
        }

    }
}
