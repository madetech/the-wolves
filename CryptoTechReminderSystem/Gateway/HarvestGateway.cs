using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoTechReminderSystem.Gateway
{
    public class HarvestGateway : IHarvestBillablePersonRetriever, ITimeSheetRetriever
    {
        private const string UsersApiAddress = "/api/v2/users";
        private const string TimeEntriesApiAddress = "/api/v2/time_entries";
        private readonly HttpClient _client;
        private readonly string[] _billablePersonRoles;
        private IMemoryCache _cache;
       
        public HarvestGateway(string address, string token, string accountId, string userAgent, string roles)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address), $"'{nameof(address)}' cannot be null or empty");

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), $"'{nameof(token)}' cannot be null or empty");

            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId), $"'{nameof(accountId)}' cannot be null or empty");

            _billablePersonRoles = CreateRoleArray(roles);
            _client = new HttpClient { BaseAddress = new Uri(address) };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Add("Harvest-Account-Id",accountId);
            _client.DefaultRequestHeaders.Add("User-Agent",userAgent);

            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
        
        public IList<HarvestBillablePerson> RetrieveBillablePeople()
        {
            var endPoint = $"{UsersApiAddress}?per_page=100";
            var cachePeriodDays = 1;
            
            var apiResponse = _cache.GetOrCreate(endPoint, cacheEntry => 
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(cachePeriodDays);
                cacheEntry.SetSize(1);

                return RetrieveWithPagination($"{UsersApiAddress}?per_page=100");
            });

            var users = apiResponse["users"];
            var activeBillablePeople = users.Where(user => (bool)user["is_active"] && IsBillablePerson(user));
            
            return activeBillablePeople.Select(billablePerson => new HarvestBillablePerson
                {
                    Id = (int) billablePerson["id"],
                    FirstName = billablePerson["first_name"].ToString(),
                    LastName = billablePerson["last_name"].ToString(),
                    Email = billablePerson["email"].ToString(),
                    WeeklyHours = SecondsToHours((int)billablePerson["weekly_capacity"])
                }
            ).ToList();
        }
        
        public IList<TimeSheet> RetrieveTimeSheets(DateTimeOffset dateFrom, DateTimeOffset dateTo)
        {
            var address = $"{TimeEntriesApiAddress}?from={ToHarvestApiString(dateFrom)}&to={ToHarvestApiString(dateTo)}";
            
            var cachePeriodMinutes = 1;
            
            var apiResponse = _cache.GetOrCreate(address, cacheEntry => 
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cachePeriodMinutes);
                cacheEntry.SetSize(1);
                return RetrieveWithPagination(address);
            });

            var timeSheets = apiResponse["time_entries"];
            
            return timeSheets.Select(timeSheet => new TimeSheet
                {
                    Id = (int)timeSheet["id"],
                    TimeSheetDate = timeSheet["spent_date"].ToString(),
                    UserId = (int)timeSheet["user"]["id"],
                    Hours = (float)timeSheet["hours"]
                }
            ).ToList(); 
        }
        
        private static string ToHarvestApiString(DateTimeOffset date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        
        private static string[] CreateRoleArray(string roles)
        {
            if(roles == null)
            {
                return new string[] { };
            }

            return roles.Split(',').Select(role => role.Trim()).ToArray();
        }
        
        private async Task<JObject> GetApiResponse(string address)
        {
            var response = await _client.GetAsync(address);
            
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private bool IsBillablePerson(JToken user)
        {
            return user["roles"].ToArray().Any(role => _billablePersonRoles.Contains(role.ToString()));
        }
        
        private JObject RetrieveFromEndPoint(string address)
        {
            var response = GetApiResponse(address);
            response.Wait();

            return response.Result;
        }

        private JObject RetrieveWithPagination(string address)
        {
            var apiResponse = RetrieveFromEndPoint($"{address}&page=1");
            var totalPages = (int) apiResponse["total_pages"];

            for (var page = 2; page <= totalPages ; page++)
            {
                apiResponse.Merge(RetrieveFromEndPoint($"{address}&page={page}"));
            }

            return apiResponse;
        }
        
        private static int SecondsToHours(int weeklyCapacity)
        {
            return weeklyCapacity / 3600;
        }
    }
}
