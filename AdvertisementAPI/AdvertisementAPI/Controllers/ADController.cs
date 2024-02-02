using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Numerics;

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
        public async Task<ActionResult<ADInfo>> AddAdInfo(int siteID, int adID, string url, string link)
        {
            try
            {
                ADInfo newInfo = new ADInfo()
                {
                    ID = adID, 
                    SITEID = siteID, 
                    URL = url, 
                    LINK = link
                };

                await context.Advertisements.AddAsync(newInfo);

                AdStatistic newStatistic = new AdStatistic()
                {
                    ID = adID,
                    VIEWS = 0,
                    CLICKS = 0
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

                    if(validTicket != null && ticket == phrase)
                    {
                        List<AdStatistic> statistics = context.ADStatistics.Where(i => (targetID < 0f ? i.ID != targetID : i.ID == targetID)).ToList();
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
                AdStatistic statistic = context.ADStatistics.Where(i => i.ID == adID).Single();

                context.Advertisements.Remove(info);
                context.ADStatistics.Remove(statistic);

                await context.SaveChangesAsync();
                
                return Ok();
            }
            catch { }

            return new BadRequestResult();
        }

        [HttpGet("getStatistic")]
        public async Task<ActionResult<AdStatistic>> GetADStatistic(int adID)
        {
            try
            {
                AdStatistic statistic = context.ADStatistics.Where(i => i.ID == adID).Single();

                return new ActionResult<AdStatistic>(statistic);
            }
            catch { }

            return NotFound();
        }

        [HttpGet("getAD")]
        public async Task<ActionResult<ADInfo>> GetADInfo(int adID)
        {
            try
            {
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();

                return new ActionResult<ADInfo>(info);
            }
            catch { }

            return NotFound();
        }

        [HttpGet("getImage")]
        public async Task<ActionResult> GetImage(int adID)
        {
            //ImplementAntiSpammingToken;
            try
            {
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();
                AdStatistic statistic = context.ADStatistics.Where(i => i.ID == adID).Single();
                statistic.VIEWS += 1;
                await context.SaveChangesAsync();
                //context.ADStatistics.Update(statistic);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"https://powderduck0.files.wordpress.com/2024/01/{info.URL?.ToLower()}");
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                //byte[] imageBuffer = await System.IO.File.ReadAllBytesAsync($"Content/{info.URL}");
                return File(bytes, "image/png");
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
                ADInfo info = context.Advertisements.Where(i => i.ID == adID).Single();
                AdStatistic statistic = context.ADStatistics.Where(i => i.ID == adID).Single();
                if (info.LINK != null)
                    redirect = info.LINK;
                statistic.CLICKS += 1;

                await context.SaveChangesAsync();
            }
            catch { }

            return Redirect(redirect);
        }
    }
}
