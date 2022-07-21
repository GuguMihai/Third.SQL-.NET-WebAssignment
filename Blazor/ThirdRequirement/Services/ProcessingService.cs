using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ThirdRequirement.Models;

namespace ThirdRequirement.Services
{
    public static class ProcessingService
    {
        public static void Process(IMemoryCache _memoryCache, string fn, string ln, Guid newGuid)
        {
            if (_memoryCache.TryGetValue(newGuid, out DataModel cacheValue))
            {
                //randomly fail
                if (DateTime.UtcNow.Second % 2 == 0)
                {
                    cacheValue.Status = "failed";
                }
                else
                {
                    cacheValue.Status = "completed";
                    cacheValue.Result = GetDbData(fn, ln);
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                _memoryCache.Set(newGuid, cacheValue, cacheEntryOptions);
            }
        }

        public static string GetDbData(string fn, string ln)
        {
            var tempRes = "";
            using (var connection = new SqlConnection(@"Data Source=localhost;Initial Catalog=Database;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=600;Encrypt=False;TrustServerCertificate=False"))
            {
                try
                {
                    connection.Open();
                    SqlDataReader reader = new SqlCommand("[dbo].[GetFinalRes] @firstName = '" + fn + "', @lastName= '" + ln + "'", connection).ExecuteReader();
                    while (reader.Read())
                    {
                        tempRes += reader[0].ToString() + " ";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    tempRes = "An error occured while processing data on stored procedure.";
                }
            }

            return tempRes;
        }
    }
}
