using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AutoLearning
{
    /// <summary>
    /// 文 件 名：Util
    /// 创建人员：谭翔
    /// 创建时间：2018/10/20 17:53:06
    /// 说明描述：
    /// 修改记录：
    /// <para>R1、……</para>
    /// <para>R2、……</para>
    /// </summary>
    public class Util
    {

        // Methods
        public static CookieContainer AddCookieToContainer(string cookie, string domian)
        {
            CookieContainer container = new CookieContainer();
            char[] separator = new char[] { ';' };
            string[] strArray = cookie.Split(separator);
            string str = null;
            int length = 0;
            string name = null;
            string str3 = null;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(strArray[i]))
                {
                    str = strArray[i];
                    length = str.IndexOf("=");
                    if (length != -1)
                    {
                        name = str.Substring(0, length).Trim();
                        if (length == (str.Length - 1))
                        {
                            str3 = "";
                        }
                        else
                        {
                            str3 = str.Substring(length + 1, (str.Length - length) - 1).Trim();
                        }
                    }
                    else
                    {
                        name = str.Trim();
                        str3 = "";
                    }
                    container.Add(new Cookie(name, str3, "", domian));
                }
            }
            return container;
        }

        private static HttpWebRequest GetHttpWebRequest(string url, string body, CookieContainer cookies, string httpMethod)
        {
            GC.Collect();
            string requestUriString = url;
            if ((httpMethod.ToUpper() == "GET") && (body.Length > 0))
            {
                requestUriString = $"{url}?{body}";
            }
            HttpWebRequest request = WebRequest.Create(requestUriString) as HttpWebRequest;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.KeepAlive = true;
            request.Method = httpMethod;
            request.Proxy = null;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:65.0) Gecko/20100101 Firefox/65.0";
            request.Timeout = 0x4e20;
            request.AllowAutoRedirect = false;
            request.CookieContainer = cookies;
            if (httpMethod.ToUpper() == "POST")
            {
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream stream = request.GetRequestStream())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(body);
                    stream.Write(bytes, 0, bytes.Length);
                }
                request.CookieContainer = new CookieContainer();
            }
            return request;
        }

        public static double GetRandomDouble() =>
            new Random().NextDouble();

        public static int GetRandomInt() =>
            new Random().Next(10000, 99999);

        public static string HandleHtml(string html)
        {
            html = html.ToLower();
            html = Regex.Replace(html, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<link[\s\S]*?>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<img[\s\S]*?>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!--[\s\S]*?-->", "", RegexOptions.IgnoreCase);
            return html;
        }

        public static void HttpNoReturn(string url, string body, CookieContainer cookies, string method = "POST")
        {
            try
            {
                HttpWebRequest request = GetHttpWebRequest(url, body, cookies, method);
                request.GetResponse();
                request.Abort();
            }
            catch
            {
            }
        }

        public static Tuple<Image, CookieContainer> HttpRtImg(string url, string body, CookieContainer cookies, string method = "Get")
        {
            try
            {
                Image img = null;
                CookieContainer container = new CookieContainer();
                HttpWebRequest request = GetHttpWebRequest(url, body, cookies, method);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    if (responseStream == null)
                        return null;
                    using (Bitmap bitmap = new Bitmap(responseStream))
                    {
                        img = bitmap.GetThumbnailImage(120, 40, null, IntPtr.Zero);
                    }

                    char[] separator = new char[] { ';' };
                    string[] strArray = response.GetResponseHeader("Set-Cookie").Split(separator);
                    int length = 0;
                    string cookieHeader = "";
                    foreach (string str3 in strArray)
                    {
                        if (str3.Length > length)
                        {
                            length = str3.Length;
                            cookieHeader = str3;
                        }
                    }
                    container.SetCookies(new Uri($"{request.RequestUri.Scheme}://{request.RequestUri.Host}"), cookieHeader);

                    responseStream.Dispose();
                }
                request.Abort();
                return Tuple.Create(img, container);
            }
            catch
            {
                return Tuple.Create<Image, CookieContainer>(null, null);
            }
        }

        public static string HttpRtHtml(string url, string body, CookieContainer cookies, string method = "POST")
        {
            try
            {
                HttpWebRequest request = GetHttpWebRequest(url, body, cookies, method);
                string str = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    if (responseStream == null)
                        return "";
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        str = reader.ReadToEnd();
                    }
                    responseStream.Dispose();
                }
                request.Abort();
                return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static CookieContainer HttpRtCookie(string url, string body, string method = "POST", CookieContainer cookies = null)
        {
            try
            {
                HttpWebRequest request = GetHttpWebRequest(url, body, cookies, method);
                CookieContainer container = new CookieContainer();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    char[] separator = new char[] { ';' };
                    string[] strArray = response.GetResponseHeader("Set-Cookie").Split(separator);
                    int length = 0;
                    string cookieHeader = "";
                    foreach (string str3 in strArray)
                    {
                        if (str3.Length > length)
                        {
                            length = str3.Length;
                            cookieHeader = str3;
                        }
                    }
                    container.SetCookies(new Uri($"{request.RequestUri.Scheme}://{request.RequestUri.Host}"), cookieHeader);
                }
                request.Abort();
                return container;
            }
            catch
            {
                return null;
            }
        }

        public static string UrlDecode(string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            return HttpUtility.UrlDecode(text, encoding);
        }

        public static string UrlEncode(string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            return HttpUtility.UrlEncode(text, encoding);
        }

        public static TEntity JsonToObject<TEntity>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TEntity));
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return (TEntity)serializer.ReadObject(mStream);
        }

        public static string RegexValue(string text, string pattern)
        {
            return Regex.Match(text, pattern).Value;
        }
    }
}
