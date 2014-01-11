namespace PushNotificationHelper
{
    using System;
    using System.Text;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;

    public class NotificationManager
    {
        private const string MediaType = "text/xml";
        private const string XWnsType = "X-WNS-Type";
        private const string AuthHeader = "Bearer";                    
        private const string AuthWwwHeader = "WWW-Authenticate";
        private const string TokenCheck = "Token expired";
        private const string AuthQuery =
            "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com";
        private const string XFormEncoded = "application/x-www-form-urlencoded";
        private const string AuthUri = "https://login.live.com/accesstoken.srf";
        private const string Success = "SUCCESS: {0}";
        private const string Failure = "ERROR: {0}";
        private const string TokenExpired = "Token expired and maximum retries reached.";
            
        private static string token;

        public static async Task<string> PostToWns(string secret, string sid, string uri, string xml, string notificationType, string contentType)
        {
            var attempts = 0;
            while (attempts++ <= 3)
            {
                attempts++;
                try
                {
                    // You should cache this access token.
                    var accessToken = token = token ?? await GetAccessToken(secret, sid);

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(xml)) };
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
                    requestMessage.Headers.Add(XWnsType, contentType);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(AuthHeader, accessToken);

                    var responseMessage = await new HttpClient().SendAsync(requestMessage);
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    attempts = 0;
                    return Message(result, true);
                }
                catch (WebException webException)
                {
                    var exceptionDetails = webException.Response.Headers[AuthWwwHeader];
                    if (exceptionDetails.Contains(TokenCheck))
                    {
                        token = null; // we can try again                 
                    }
                    else
                    {
                        return Message(webException.Message, false);
                    }
                }
                catch (Exception ex)
                {
                    return Message(ex.Message, false);
                }              
            }

            return Message(TokenExpired, false);                
        }

        private static string Message(string message, bool success)
        {
            return string.Format(success ? Success : Failure, message);
        }

        private static OAuthToken GetOAuthTokenFromJson(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var ser = new DataContractJsonSerializer(typeof(OAuthToken));
                var oAuthToken = (OAuthToken)ser.ReadObject(ms);
                return oAuthToken;
            }
        }

        protected static async Task<string> GetAccessToken(string secret, string sid)
        {
            var urlEncodedSecret = WebUtility.UrlEncode(secret);
            var urlEncodedSid = WebUtility.UrlEncode(sid);

            var body = String.Format(AuthQuery,
                                     urlEncodedSid,
                                     urlEncodedSecret);

            string response;
            using (var client = new HttpClient())
            {
                var httpBody = new StringContent(body, Encoding.UTF8, XFormEncoded);
                var httpResponse = await client.PostAsync(AuthUri, httpBody);
                response = await httpResponse.Content.ReadAsStringAsync();
            }
            return GetOAuthTokenFromJson(response).AccessToken;
        }

    }
}
