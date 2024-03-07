using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdvertisementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ADController : ControllerBase
    {
        private AdContext context;

        public ADController(AdContext dbContext)
        {
            context = dbContext;
        }

        [HttpGet("addAD")]
        public async Task<ActionResult<ADInfo>> AddAdInfo(int siteID, int adID, string client, string url, string link)
        {
            try
            {
                ADInfo newInfo = new ADInfo()
                {
                    ID = adID,
                    SITEID = siteID,
                    URL = url,
                    LINK = link, 
                    CLIENT = client
                };

                await context.Advertisements.AddAsync(newInfo);

                AdStatistic newStatistic = new AdStatistic()
                {
                    STATID = adID,
                    VIEWS = 0,
                    CLICKS = 0,
                    VALIDDATE = DateTime.UtcNow
                };

                await context.ADStatistics.AddAsync(newStatistic);

                await context.SaveChangesAsync();

                return new ActionResult<ADInfo>(newInfo);
            }
            catch { }

            return new BadRequestResult();
        }

        [HttpGet("resetStatistic")]
        public async Task<ActionResult<string>> ResetStatistic(int adID = -1, string ticket = "")
        {
            try
            {
                if(ticket == "")
                {
                    int ticketLength = 8;
                    byte[] ticketBytes = new byte[ticketLength];
                    Random randomizer = new Random();

                    for (int i = 0; i < ticketLength; i++)
                    {
                        ticketBytes[i] = (byte)randomizer.Next(65, 90);
                    }

                    string requestTicket = System.Text.Encoding.ASCII.GetString(ticketBytes);
                    using(StreamWriter writer = new StreamWriter("Ticket.txt"))
                    {
                        await writer.WriteAsync($"{requestTicket}:{adID}");
                        writer.Close();
                    }

                    return new ActionResult<string>(requestTicket);
                }
            
                using(StreamReader reader = new StreamReader("Ticket.txt"))
                {
                    string validTicket = await reader.ReadToEndAsync();
                    string phrase = validTicket.Split(':')[0];
                    int targetID = int.Parse(validTicket.Split(':')[1]);
                    DateTime currentTime = DateTime.UtcNow;

                    if(validTicket != null && ticket == phrase)
                    {
                        List<AdStatistic> statistics = context.ADStatistics.Where(i => (targetID < 0f ? i.STATID != targetID : i.STATID == targetID) && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ToList();
                        for (int i = 0; i < statistics.Count; i++)
                        {
                            statistics[i].VIEWS = 0;
                            statistics[i].CLICKS = 0;
                        }

                        await context.SaveChangesAsync();

                        return new ActionResult<string>(validTicket);
                    }

                    reader.Close();

                    return BadRequest();
                }
            }
            catch { }

            return BadRequest();
        }

        [HttpGet("removeAD")]
        public async Task<ActionResult> RemoveADInfo(int adID)
        {
            try
            {
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();
                AdStatistic[] statistic = context.ADStatistics.Where(i => i.ID == adID).ToArray();

                context.Advertisements.Remove(info);
                context.ADStatistics.RemoveRange(statistic);
                //context.ADStatistics.Remove(statistic);

                await context.SaveChangesAsync();
                
                return Ok();
            }
            catch { }

            return new BadRequestResult();
        }

        [HttpGet("graphs")]
        public async Task<ContentResult> GraphAuthentication(string username, string password)
        {
            Client? cl = await context.Clients.SingleOrDefaultAsync(i => i.Username == username && i.Password == password);

            if(cl != null)
            {
                string charter = await System.IO.File.ReadAllTextAsync("Content/Graphs/Charter.html");
                return new ContentResult() { Content = charter, ContentType = "text/html" };
            }

            return new ContentResult() { Content = "<h1> Unauthorized <h1>", ContentType = "text/html" };
        }

        [HttpGet("availableAds")]
        public async Task<ActionResult<List<int>>> GetAdIndices()
        {
            /*List<AdStatistic> stats = context.ADStatistics.GroupBy(i => i.STATID).Select(b => new AdStatistic() { STATID = b.Key }).ToList();
            List<int> indices = stats.Select(i => (int)i.STATID).ToList();*/
            List<int> indices = context.ADStatistics.GroupBy(i => i.STATID).Select(b => (int)b.Key).ToList();

            return new ActionResult<List<int>>(indices);
        }

        [HttpGet("getStatistic")]
        public async Task<ActionResult<List<AdStatistic>>> GetADStatistic(int adID = -1, int m = 0, int y = 0, int range = 1)
        {
            try
            {
                DateTime referenceDate = m <= 0f && y <= 0f ? DateTime.UtcNow : new DateTime(year: y, month: m, day: 1).ToUniversalTime();
                float months = (referenceDate.Year * 12 + referenceDate.Month);
                float rangeMonths = months - (range - 1f);

                //ConditionWhetherToAccessTheStatisticByIDORByTheTimeExplicitly;
                //List<AdStatistic> statistic = context.ADStatistics.Where(i => i.ID == adID && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month >= rangeMonths && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month <= months).OrderBy(i => i.VALIDDATE).ToList();
                List<AdStatistic> statistic = context.ADStatistics.Where(i => (adID < 0f ? i.STATID != adID : i.STATID == adID) && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month >= rangeMonths && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month <= months).OrderBy(i => i.STATID).ToList();
                
                return new ActionResult<List<AdStatistic>>(statistic);
            }
            catch { }

            return NotFound();
        }

        /*[HttpGet("getStatistic")]
        public async Task<ActionResult<List<AdStatistic>>> GetADStatistic(int adID = -1, int m = 0, int y = 0, int range = 1)
        {
            try
            {
                DateTime referenceDate = m <= 0f && y <= 0f ? DateTime.UtcNow : new DateTime(year: y, month: m, day: 1).ToUniversalTime();
                float months = (referenceDate.Year * 12 + referenceDate.Month);
                float rangeMonths = months - (range - 1f);

                //ConditionWhetherToAccessTheStatisticByIDORByTheTimeExplicitly;
                //List<AdStatistic> statistic = context.ADStatistics.Where(i => i.ID == adID && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month >= rangeMonths && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month <= months).OrderBy(i => i.VALIDDATE).ToList();
                //List<AdStatistic> statistic = context.ADStatistics.Where(i => (adID < 0f ? i.ID != adID : i.ID == adID) && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month >= rangeMonths && i.VALIDDATE.Year * 12 + i.VALIDDATE.Month <= months).ToList();//.OrderBy(i => i.ID).ToList();
                List<AdStatistic> statistic = new List<AdStatistic>();
                return new ActionResult<List<AdStatistic>>(statistic);
            }
            catch { }

            return NotFound();
        }*/

        /*[HttpGet("getStatistic")]
        public async Task<ActionResult<AdStatistic>> GetADStatistic(int adID)
        {
            try
            {
                AdStatistic statistic = context.ADStatistics.Where(i => i.ID == adID).Single();

                return new ActionResult<AdStatistic>(statistic);
            }
            catch { }

            return NotFound();
        }*/

        [HttpGet("getAD")]
        public async Task<ActionResult<ADInfo>> GetADInfo(int adID, bool count = false)
        {
            try
            {
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();

                DateTime currentTime = DateTime.UtcNow;
                List<AdStatistic> statistics = context.ADStatistics.Where(i => i.STATID == adID && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ToList();

                if (statistics.Count <= 0f)
                {
                    await context.ADStatistics.AddAsync(new AdStatistic()
                    {
                        STATID = adID,
                        VIEWS = 0,
                        CLICKS = 0,
                        VALIDDATE = DateTime.UtcNow
                    });
                    
                    await context.SaveChangesAsync();
                }

                if(count)
                    await context.ADStatistics.Where(i => i.STATID == adID && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ExecuteUpdateAsync(i => i.SetProperty(i => i.VIEWS, i => i.VIEWS + 1));

                return new ActionResult<ADInfo>(info);
            }
            catch { }

            return NotFound();
        }

        [HttpGet("getImage")]
        public async Task<ActionResult> GetImage(int adID, bool wide = true)
        {
            //ImplementAntiSpammingToken;
            try
            {
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();

                /*DateTime currentTime = DateTime.UtcNow;
                List<AdStatistic> statistics = context.ADStatistics.Where(i => i.ID == adID && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ToList();

                if (statistics.Count <= 0f)
                {
                    await context.ADStatistics.AddAsync(new AdStatistic()
                    {
                        ID = adID,
                        VIEWS = 0,
                        CLICKS = 0,
                        VALIDDATE = DateTime.UtcNow
                    });

                    await context.SaveChangesAsync();
                }

                await context.ADStatistics.Where(i => i.ID == adID && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ExecuteUpdateAsync(i => i.SetProperty(i => i.VIEWS, i => i.VIEWS + 1));
                */
                /*HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"https://powderduck0.files.wordpress.com/2024/01/{info.URL?.ToLower()}");
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();*/
                string[] dimensions = new string[] { "Wide", "" };
                int index = wide ? 0 : 1;
                string[] imageNameDivisions = info.URL.Split('.');
                string name = "";

                for (int i = 0; i < imageNameDivisions.Length - 1; i++)
                {
                    name += imageNameDivisions[i];
                }

                //byte[] imageBuffer = await System.IO.File.ReadAllBytesAsync($"Content/{info.URL}");
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        string dimensionType = dimensions[(index + i) % 2];
                        string extension = $"{name}{dimensionType}.{imageNameDivisions[imageNameDivisions.Length - 1]}";
                        byte[] imageBuffer = await System.IO.File.ReadAllBytesAsync($"Content/{extension}");
                        return File(imageBuffer, "image/png");
                    }
                    catch { }
                }
                //byte[] imageBuffer = await System.IO.File.ReadAllBytesAsync($"Content/{name}");
                //return File(imageBuffer, "image/png");
                //return File(bytes, "image/png");
                return NotFound();
            }
            catch { }

            return NotFound();
        }

        [HttpGet("adClicked")]
        public async Task<ActionResult<string>> AdClick(int adID)
        {
            //ImplementAntiSpammingToken;
            string redirect = Request.Headers.Referer.ToString();
            try
            {
                DateTime currentTime = DateTime.UtcNow;
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();
                List<AdStatistic> statistics = context.ADStatistics.Where(i => i.STATID == adID && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ToList();

                if (info.LINK != null)
                    redirect = info.LINK;

                if(statistics.Count <= 0f)
                {
                    await context.ADStatistics.AddAsync(new AdStatistic()
                    {
                        STATID = adID, 
                        VIEWS = 0,
                        CLICKS = 0,
                        VALIDDATE = DateTime.UtcNow
                    });

                    await context.SaveChangesAsync();
                }

                await context.ADStatistics.Where(i => i.STATID == adID && i.VALIDDATE.Year == currentTime.Year && i.VALIDDATE.Month == currentTime.Month).ExecuteUpdateAsync(i => i.SetProperty(i => i.CLICKS, i => i.CLICKS + 1));
            }
            catch { }

            return Redirect(redirect);
        }
    }
}
