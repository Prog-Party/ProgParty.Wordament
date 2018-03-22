using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WordamentHelper
{
    public class WordCheckOnline
    {
        public List<String> CheckWordOnline(String word)
        {
            String url = "http://www.woorden.org/autoComplete/rpc.php";

            String html = GetWebsite(url, word);
            List<String> results = GetResultsByHtml(html);
            return results;
        }

        private List<string> GetResultsByHtml(string html)
        {
            List<String> result = new List<string>();
            
            foreach (var d in html.Replace("<br>", "|").Split('|'))
            {
                var pattern = @"<(a)[^>]*>(?<content>[^<]*)<";
                var regex = new Regex(pattern);
                var m = regex.Match(d);
                if ( m.Success ) 
                  result.Add(m.Groups["content"].Value.Trim());
            }

            return result;
        }

        public String GetWebsite(String url, String word)
        {
            HttpWebResponse response = null;
            StreamReader reader = null;
            String result = String.Empty;
            int responseInt = 0;
            try
            {
                word = "woord=" + word;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] dataBytes = encoding.GetBytes(word);

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 5000;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = dataBytes.Length;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0";
                
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(dataBytes, 0, dataBytes.Length);
                    dataStream.Close();
                }

                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                String message = String.Format("{0}, ERROR, response code: {1}", e.Message, responseInt);
                throw new Exception(message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }

            return result;
        }
    }
}
