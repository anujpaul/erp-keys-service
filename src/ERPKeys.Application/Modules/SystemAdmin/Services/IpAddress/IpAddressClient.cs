using ERPKeys.Application.Modules.SystemAdmin.Services.ExternalClients.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace ERPKeys.Application.Modules.SystemAdmin.Services.ExternalClients
{
    //    {
    //    "status": "success",
    //    "country": "United States",
    //    "countryCode": "US",
    //    "region": "WI",
    //    "regionName": "Wisconsin",
    //    "city": "Madison",
    //    "zip": "53719",
    //    "lat": 43.0344,
    //    "lon": -89.5007,
    //    "timezone": "America/Chicago",
    //    "isp": "Charter Communications",
    //    "org": "Spectrum",
    //    "as": "AS20115 Charter Communications LLC",
    //    "query": "131.93.244.88"
    //     }
    public interface IIpAddressClient
    {
        public Task<string> GetCityAsync(string ipAddress);
    }
    public class IpAddressClient : IIpAddressClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<IpAddressClient> _logger;
                
        public IpAddressClient(HttpClient http, ILogger<IpAddressClient> logger)
        {
            _http = http;
            _logger = logger;
            
        }

        public async Task<string> GetCityAsync(string ipAddress)
        {
            try
            {

                var response = await _http.GetAsync($"http://ip-api.com/json/{ipAddress}");

                if (!response.IsSuccessStatusCode)
                    return "city";

                var result = await response.Content.ReadFromJsonAsync<IpGeoLocationResponseDto>();

                if (result is null || !string.Equals(result.Status, "success", StringComparison.OrdinalIgnoreCase))
                    return "Developer, Local";

                return $"{result.city}, {result.region}, {result.country}";

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Something bad happened {ex.Message}");
                return "failed";
            }
        }


    }
}
