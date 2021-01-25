using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace CideClient.Api
{
    public class CideApi
    {
        static public dynamic post(string json)
        {
            var client = new RestClient("http://0.0.0.0:5000/api/FleetCarrierItem");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json,  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }
            else
            {
                return "error";
            }

        }
        static public dynamic put(string json, string id)
        {
            var client = new RestClient($"http://0.0.0.0:5000/api/FleetCarrierItem/{id}");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json,  ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK | response.StatusCode == HttpStatusCode.NoContent)
            {
                return response.Content;
            }
            else
            {
                return "error";
            }

        }
        static public dynamic delete(string id)
        {
            return null;
        }
        static public dynamic get(string id)
        {
            var client = new RestClient($"http://0.0.0.0:5000/api/FleetCarrierItem/{id}");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }
            else
            {
                return "error";
            }
            
        }
        static public dynamic getall()
        {
            return null;
        }

    }
}