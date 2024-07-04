
using LibCommon;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Cache;


namespace Luxriot.Internal.LibCommon
{

    public class DigestWebRequest
    {

        readonly string _user;
        readonly string _password;

        string _realm;
        string _nonce;
        string _qop;
        string _cnonce;
        string _opaque;
        DateTime _cnonceDate;
        int _nc;
        public HttpWebRequest request;
        // ----------------------------------------------------------------------


        public DigestWebRequest(string user, string password)
        {
            _user = user;
            _password = password;
        }
        // ----------------------------------------------------------------------

        public DigestWebRequest(string user, string password, string realm)
        {
            _user = user;
            _password = password;
            _realm = realm;
        }
        // ----------------------------------------------------------------------

        public string Method { get; set; } = WebRequestMethods.Http.Get;
        // ----------------------------------------------------------------------

        public string ContentType { get; set; }
        // ----------------------------------------------------------------------

        public byte[] PostData { get; set; }
        // ----------------------------------------------------------------------

        public Action<HttpWebRequest> Configurator { get; set; } = null;
        // ----------------------------------------------------------------------

        public HttpWebResponse GetResponse(Uri uri)
        {

            HttpWebResponse response = null;
            int counter = 0;
            int max_attempts = 2;

            while ((response == null || response.StatusCode != HttpStatusCode.Accepted) && counter < max_attempts)
            {

                try
                {

                    this.request = CreateHttpWebRequestObject(uri);

                    if (!string.IsNullOrEmpty(_cnonce) && DateTime.Now.Subtract(_cnonceDate).TotalHours < 1.0)
                        request.Headers.Add("Authorization", ComputeDigestHeader(uri));


                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch (WebException ex)
                    {

                        if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                        {

                            var www_auth_header = ex.Response.Headers["WWW-Authenticate"];
                            _realm = GetDigestHeaderAttribute("realm", www_auth_header);
                            _nonce = GetDigestHeaderAttribute("nonce", www_auth_header);
                            _qop = GetDigestHeaderAttribute("qop", www_auth_header);
                            _opaque = GetDigestHeaderAttribute("opaque", www_auth_header);

                            _nc = 0;
                            _cnonce = new Random().Next(123400, 9999999).ToString();
                            _cnonceDate = DateTime.Now;

                            request = CreateHttpWebRequestObject(uri, true);

                            counter++;
                            response = (HttpWebResponse)request.GetResponse();
                        }
                        else
                        {
                            throw ex;
                        }
                    }

                    switch (response.StatusCode)
                    {

                        case HttpStatusCode.OK:
                        case HttpStatusCode.Accepted:
                            return response;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.Moved:

                            uri = new Uri(response.Headers["Location"]);
                            counter--; // Decrement the loop counter, as there might be a variable number of redirections which we should follow
                            break;
                    }
                }
                catch (WebException ex) { throw ex; }
            }
            throw new Exception("Error: Either authentication failed, authorization failed or the resource doesn't exist");
        }
        // ----------------------------------------------------------------------

        HttpWebRequest CreateHttpWebRequestObject(Uri uri, bool addAuthenticationHeader)
        {

            this.request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = true;
            request.PreAuthenticate = true;
            request.Method = Method;
            request.Timeout = 180000;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);
            request.CachePolicy = noCachePolicy;
            request.ContentType = "image/jpeg";


            Configurator?.Invoke(request);

            if (!string.IsNullOrEmpty(ContentType))
                request.ContentType = ContentType;

            if (addAuthenticationHeader)
                request.Headers.Add("Authorization", ComputeDigestHeader(uri));

            request.KeepAlive = false;

            if (PostData != null && PostData.Length > 0)
            {

                request.ContentLength = PostData.Length;
                var post_data_stream = request.GetRequestStream(); //open connection
                post_data_stream.Write(PostData, 0, PostData.Length); // Send the data.
                post_data_stream.Close();
            }
            else if (Method == WebRequestMethods.Http.Post && (PostData == null || PostData.Length == 0))
            {
                request.ContentLength = 0;
            }
            return request;
        }
        // ----------------------------------------------------------------------

        HttpWebRequest CreateHttpWebRequestObject(Uri uri) => CreateHttpWebRequestObject(uri, false);
        // ----------------------------------------------------------------------

        string ComputeDigestHeader(Uri uri)
        {

            _nc = _nc + 1;

            var ha1 = ComputeMd5Hash("{0}:{1}:{2}".W(_user, _realm, _password));
            var ha2 = ComputeMd5Hash("{0}:{1}".W(Method, uri.PathAndQuery));
            var digest_response = ComputeMd5Hash("{0}:{1}:{2:00000000}:{3}:{4}:{5}".W(ha1, _nonce, _nc, _cnonce, _qop, ha2));

            return ("Digest username=\"{0}\",realm=\"{1}\",nonce=\"{2}\",uri=\"{3}\"," +
                   "cnonce=\"{7}\",nc={6:00000000},qop={5},response=\"{4}\",opaque=\"{8}\"").W(
                _user, _realm, _nonce, uri.PathAndQuery, digest_response, _qop, _nc, _cnonce, _opaque);
        }
        // ----------------------------------------------------------------------

        string GetDigestHeaderAttribute(string attributeName, string digestAuthHeader)
        {

            Match m;
            var header_re = new Regex(@"{0}=""([^""]*)""".W(attributeName));
            if ((m = header_re.Match(digestAuthHeader)).Success)
                return m.Groups[1].Value;
            throw new ApplicationException("Header {0} not found".W(attributeName));
        }
        // ----------------------------------------------------------------------

        string ComputeMd5Hash(string input)
        {

            var in_bytes = Encoding.ASCII.GetBytes(input);
            var hash = MD5.Create().ComputeHash(in_bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        // ----------------------------------------------------------------------
    }
}