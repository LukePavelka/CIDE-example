using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace CideClient.Api
{
    public class PlotterSpansh
    {
        static public string NewRoutes(string Source, string Destination, string CapacityUsed, string FuelLoaded, string TritiumStored)
        {
            var client = new RestClient($"https://spansh.co.uk/api/fleetcarrier/route/?source={Source}&destination={Destination}&capacity_used={CapacityUsed}&fuel_loaded={FuelLoaded}&tritium_stored={TritiumStored}");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                return response.Content;
            }
            else
            {
                return "error";
            }
        }
        static public string Result(string Id)
        {
            var client = new RestClient($"https://spansh.co.uk/api/results/{Id}");
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }
            else if(response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "CantFindRoute";
            }
            else
            {
                return Result(Id);
            }
        }
    }
}