using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace PeopleViewer
{
    public record Person(int ID, string GivenName, string FamilyName,
        DateTime StartDate, int Rating, string FormatString)
    {
        public override string ToString()
        {
            if (string.IsNullOrEmpty(FormatString))
                return $"{GivenName} {FamilyName}";
            return string.Format(FormatString, GivenName, FamilyName);
        }
    }

    public static class PersonReader
    {
        private static string BaseAddress = "http://localhost:9874" ;
        public static JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static List<int> GetIds()
        {
            WebClient client = new WebClient();
            string address = $"{BaseAddress}/people/ids";
            string reply = client.DownloadString(address);
            try
            {
                return JsonSerializer.Deserialize<List<int>>(reply, options);
            }
            catch (Exception)
            {
                throw new HttpRequestException($"Error parsing data from ({address}): {reply}");
            }
        }

        public static Person GetPerson(int id)
        {
            WebClient client = new WebClient();
            string address = $"{BaseAddress}/people/{id}";
            string reply = client.DownloadString(address);
            try
            {
                return JsonSerializer.Deserialize<Person>(reply, options);
            }
            catch (Exception)
            {
                throw new HttpRequestException($"Error parsing data from ({address}): {reply}");
            }
        }
    }
}
