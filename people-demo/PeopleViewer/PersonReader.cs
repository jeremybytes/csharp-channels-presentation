using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
        private static HttpClient client = new() { BaseAddress = new Uri("http://localhost:9874") };
        private static JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static async Task<List<int>> GetIdsAsync()
        {
            HttpResponseMessage response =
                await client.GetAsync("people/ids").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var stringResult =
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<List<int>>(stringResult);
            }
            throw new HttpRequestException($"Unable to complete request: status code {response.StatusCode}");
        }

        public static async Task<Person> GetPersonAsync(int id)
        {
            HttpResponseMessage response =
                await client.GetAsync($"people/{id}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var stringResult =
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<Person>(stringResult, options);
            }
            throw new HttpRequestException($"Unable to complete request: status code {response.StatusCode}");
        }
    }
}
