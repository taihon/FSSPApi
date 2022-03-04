using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSSPAPI.Core
{
    public class APIExchanger
    {
        private string _apiAddress;
        private string _apiKey;
        private static HttpClient _client;
        public APIExchanger()
        {
            _apiAddress = "https://api-ip.fssp.gov.ru/api/v1.0";
            _apiKey = "FILL_ME";
            ServicePointManager.SecurityProtocol =SecurityProtocolType.Tls13;
            ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;
            //ssl hacks below
            /*
            var sslOptions = new SslClientAuthenticationOptions
            {
                CipherSuitesPolicy = new CipherSuitesPolicy(new List<TlsCipherSuite> { TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384 })
            };
            var socketHandler = new SocketsHttpHandler { SslOptions = sslOptions };
            //end ssl hacks
            _client = new HttpClient(socketHandler,true);
            */
            _client = new HttpClient();
        }
        public async Task<string> SendGroupRequestAsync(List<Person> input)
        {
                var groupRequest = new GroupRequest()
            {
                token = _apiKey,
                request = new List<SingleRequest>()
            };
            foreach(var p in input)
            {
                groupRequest.request.Add(
                    new SingleRequest() { 
                        Params = new PhysicalParams(p) });
            }
            var jsonBody = JsonConvert.SerializeObject(groupRequest);
            var cont = new StringContent(jsonBody,Encoding.UTF8,"application/json");
            var response = await _client.PostAsync($"{_apiAddress}/search/group", cont);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return string.Empty;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            try
            {
                var task = JsonConvert.DeserializeObject<GroupRequestResult>(responseString);
                if (task.Status == "success")
                {
                    return task.Response.Task;
                }
                else { return string.Empty; }
            }
            catch { 
                return string.Empty; 
            }
        }
        public async Task<bool> GroupRequestStatusIsCompleted(string guid)
        {
            var url = $"{_apiAddress}/status?token={HttpUtility.UrlEncode(_apiKey)}&task={HttpUtility.UrlEncode(guid)}";
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseTemplate = new { Status = "", Code = 0, Exception = "", Response = new { Status = 0, Progress = ""} };
            var result = JsonConvert.DeserializeAnonymousType(responseString, responseTemplate);
            if (string.IsNullOrEmpty(result.Exception))
            {
                if(result.Response.Status == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<List<PhysicalResponse>> FetchReadyGroupRequest(string guid)
        {
            var url = $"{_apiAddress}/result?token={HttpUtility.UrlEncode(_apiKey)}&task={HttpUtility.UrlEncode(guid)}";
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GroupRequestResultResponse>(responseString);
            return result.Response.Result.SelectMany(r => r.Result).ToList();
        }
    }
}
