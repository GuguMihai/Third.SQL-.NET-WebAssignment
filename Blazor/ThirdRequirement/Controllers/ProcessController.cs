using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Threading;
using ThirdRequirement.Models;
using ThirdRequirement.Services;

namespace ThirdRequirement.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public ProcessController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("StartCalculation/{fn}/{ln}")]
        public Guid StartCalculation(string fn, string ln)
        {
            var newGuid = Guid.NewGuid();
            var rng = new Random();
            var calcTime = rng.Next(20, 60); // seconds

            DataModel m = new DataModel()
            {
                Id = newGuid,
                ComputationStartDatetime = DateTime.UtcNow,
                ComputationRequiredTime = calcTime,
                Status = "running",
                FirstName = fn,
                LastName = ln
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
            _memoryCache.Set(newGuid, m, cacheEntryOptions);

            if (!_memoryCache.TryGetValue("allProcesses", out List<Guid> allProc))
            {
                allProc = new List<Guid>();
            }

            allProc.Add(newGuid);
            _memoryCache.Set("allProcesses", allProc, cacheEntryOptions);

            int interval = calcTime * 1000; // milisec
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(interval);
                ProcessingService.Process(_memoryCache, fn, ln, newGuid);
            });

            return newGuid;
        }

        [HttpGet]
        [Route("GetStatus/{id}")]
        public string GetStatus(Guid id)
        {
            var res = new StatusObject();
            if (_memoryCache.TryGetValue(id, out DataModel model))
            {
                if (model.Status == "running")
                {
                    var elapsedDateTime = DateTime.UtcNow - model.ComputationStartDatetime;
                    var elapsedSec = elapsedDateTime.Seconds;
                    res.Progress = (int)Math.Round((double)(100 * elapsedSec) / model.ComputationRequiredTime);
                    res.Status = model.Status;
                }
                else if (model.Status == "failed")
                {
                    res.Progress = 100;
                    res.Status = model.Status;
                }
                else if (model.Status == "completed")
                {
                    res.Progress = 100;
                    res.Result = model.Result;
                    res.Status = model.Status;

                    if(res.Result == null)
                    {
                        res.Result = ProcessingService.GetDbData(model.FirstName, model.LastName);
                    }
                }
            }
            else
            {
                res.Error = "Id not found in list";
            }

            return JsonSerializer.Serialize(res);
        }

        [HttpGet]
        [Route("GetAll")]
        public string GetAllProcesses(string fn, string ln)
        {
            var res = new List<DataModel>();
            _memoryCache.TryGetValue("allProcesses", out List<Guid> allProc);

            foreach (var p in allProc)
            {
                _memoryCache.TryGetValue(p, out DataModel model);

                res.Add(model);
            }

            return JsonSerializer.Serialize(res); ;
        }
    }
}
