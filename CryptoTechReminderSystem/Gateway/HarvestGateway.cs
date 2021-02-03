using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CryptoTechReminderSystem.DomainObject;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Polly;

namespace CryptoTechReminderSystem.Gateway
{
    public class HarvestGateway : IHarvestBillablePersonRetriever, ITimeSheetRetriever
    {
        private const string UsersApiAddress = "/api/v2/users";
        private const string TimeEntriesApiAddress = "/api/v2/time_entries";
        private const string ProjectsApiAddress = "/api/v2/projects";
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
            var address = $"{UsersApiAddress}?per_page=100";
            
            var apiResponse = GetFromCacheOrAPI(address, cachePeriodInMinutes: 180, cacheEntrySize: 1);

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
            
            var apiResponse = GetFromCacheOrAPI(address, cachePeriodInMinutes: 1, cacheEntrySize: 1);

            var timeSheets = apiResponse["time_entries"];
            
            return timeSheets.Select(timeSheet => new TimeSheet
                {
                    Id = (int)timeSheet["id"],
                    TimeSheetDate = timeSheet["spent_date"].ToString(),
                    UserId = (int)timeSheet["user"]["id"],
                    Hours = (float)timeSheet["hours"],
                    IsClosed = (bool)timeSheet["is_closed"],
                    ProjectManagerIds = GetProjectManagerIds((int)timeSheet["project"]["id"])
                }
            ).ToList();
        }
        private List<int> GetProjectManagerIds(int projectId) {
            var address = $"{ProjectsApiAddress}/{projectId}/user_assignments";
            var apiResponse = GetFromCacheOrAPI(address, 180, 1);
            var userAssignments = apiResponse["user_assignments"];

            var projectManagerIds = userAssignments
                .Where(userAssignment => (bool) userAssignment["is_project_manager"] == true)
                .Select(userAssignment => (int) userAssignment["user"]["id"]).ToList();
            return projectManagerIds;
        }

        private JObject GetFromCacheOrAPI(string apiAddress, int cachePeriodInMinutes, int cacheEntrySize) {
            return _cache.GetOrCreate(apiAddress, cacheEntry => 
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cachePeriodInMinutes);
                cacheEntry.SetSize(cacheEntrySize);
                return RetrieveWithPagination(apiAddress);
            });
        }

        private async Task<JObject> GetApiResponse(string address)
        {   
            var harvestRetryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(15),
                });
            
            var response = await harvestRetryPolicy.ExecuteAsync(() => (_client.GetAsync(address)));
                
            return JObject.Parse(await response.Content.ReadAsStringAsync());
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
            var apiResponse = RetrieveFromEndPoint(AppendURLWithPageNumber(address, 1));
            var totalPages = (int) apiResponse["total_pages"];

            if (totalPages == 1) return apiResponse;
            
            for (var pageNumber = 2; pageNumber <= totalPages; pageNumber++)
            {
                apiResponse.Merge(RetrieveFromEndPoint(AppendURLWithPageNumber(address, pageNumber)));
            }

            return apiResponse;
        }

        private string AppendURLWithPageNumber(string address, int pageNumber)
        {
            return address + (HasOtherParameters(address) ? "&" : "?") + "page=" + pageNumber;
        }
        private bool HasOtherParameters(string address)
        {
            return address.Contains("?");
        }
        
        private static int SecondsToHours(int weeklyCapacity)
        {
            return weeklyCapacity / 3600;
        }
    }
}
