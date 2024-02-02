using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace AdvertisementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdsController : ControllerBase
    {
        private string connectionString = "";

        public AdsController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("SQLConnection");
        }

        [HttpGet("addAD")]
        public async Task<ActionResult<ADInfo>> AddAdInfo(int siteID, int adID, string url, string link)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using(SqlCommand insertQuery = new SqlCommand("INSERT INTO Advertisements VALUES (@id, @url, @link, @siteID);", connection))
                    {
                        insertQuery.Parameters.AddWithValue("@id", adID);
                        insertQuery.Parameters.AddWithValue("@url", url);
                        insertQuery.Parameters.AddWithValue("@link", link);
                        insertQuery.Parameters.AddWithValue("@siteID", siteID);

                        await insertQuery.ExecuteNonQueryAsync();
                    }
                    
                    using (SqlCommand secondInsertQuery = new SqlCommand("INSERT INTO ADStatistics VALUES (@id, @views, @clicks);", connection))
                    {
                        secondInsertQuery.Parameters.AddWithValue("@id", adID);
                        secondInsertQuery.Parameters.AddWithValue("@views", 0);
                        secondInsertQuery.Parameters.AddWithValue("@clicks", 0);

                        await secondInsertQuery.ExecuteNonQueryAsync();
                    }
                }
                
                return new ActionResult<ADInfo>(new ADInfo()
                {
                    ID = adID, 
                    URL = url, 
                    LINK = link
                });
            }
            catch { }

            return new BadRequestResult();
        }

        [HttpGet("getStatistic")]
        public async Task<ActionResult<AdStatistic>> GetADStatistic(int adID)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using (SqlCommand infoQuery = new SqlCommand("SELECT * FROM ADStatistics WHERE ID = @adID;", connection))
                {
                    infoQuery.Parameters.AddWithValue("@adID", adID);

                    using (SqlDataReader adReader = await infoQuery.ExecuteReaderAsync())
                    {
                        await adReader.ReadAsync();

                        AdStatistic adResult = new AdStatistic()
                        {
                            ID = (int)adReader["ID"], 
                            VIEWS = (int)adReader["VIEWS"], 
                            CLICKS = (int)adReader["CLICKS"]
                        };

                        await connection.CloseAsync();

                        return new ActionResult<AdStatistic>(adResult);
                    }
                }
            }
            catch { }

            return NotFound();
        }

        [HttpGet("getAD")]
        public async Task<ActionResult<ADInfo>> GetADInfo(int adID)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using (SqlCommand infoQuery = new SqlCommand("SELECT * FROM Advertisements WHERE ID = @adID;", connection))
                {
                    infoQuery.Parameters.AddWithValue("@adID", adID);

                    using (SqlDataReader adReader = await infoQuery.ExecuteReaderAsync())
                    {
                        await adReader.ReadAsync();

                        ADInfo adResult = new ADInfo()
                        {
                            ID = (int)adReader["ID"],
                            URL = (string)adReader["URL"],
                            LINK = (string)adReader["LINK"]
                        };

                        await connection.CloseAsync();

                        return new ActionResult<ADInfo>(adResult);
                    }
                }
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
                string imageURL = "";
                SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using (SqlCommand imageQuery = new SqlCommand("SELECT URL FROM Advertisements WHERE ID = @adID;", connection))
                {
                    imageQuery.Parameters.AddWithValue("@adID", adID);
                    using (SqlDataReader reader = await imageQuery.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        imageURL = (string)reader["URL"];
                    }
                }

                int currentViews = 0;
                using (SqlCommand currentViewQuery = new SqlCommand("SELECT VIEWS FROM ADStatistics WHERE ID = @adID;", connection))
                {
                    currentViewQuery.Parameters.AddWithValue("@adID", adID);
                    using (SqlDataReader viewReader = await currentViewQuery.ExecuteReaderAsync())
                    {
                        await viewReader.ReadAsync();
                        currentViews = (int)viewReader["VIEWS"];
                    }
                }

                using (SqlCommand viewQuery = new SqlCommand("UPDATE ADStatistics SET VIEWS = @views WHERE ID = @adID;", connection))
                {
                    viewQuery.Parameters.AddWithValue("@adID", adID);
                    viewQuery.Parameters.AddWithValue("@views", currentViews + 1);
                    await viewQuery.ExecuteNonQueryAsync();
                }

                //string url = string.Join("/", "Content", imageURL);
                byte[] imageBuffer = await System.IO.File.ReadAllBytesAsync($"Content/{imageURL}");
                return File(imageBuffer, "image/png");
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
                int currentClicks = 0;
                SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using(SqlCommand linkQuery = new SqlCommand("SELECT LINK FROM Advertisements WHERE ID = @adID;", connection))
                {
                    linkQuery.Parameters.AddWithValue("@adID", adID);
                    using(SqlDataReader linkReader = await linkQuery.ExecuteReaderAsync())
                    {
                        await linkReader.ReadAsync();

                        redirect = (string)linkReader["LINK"];
                    }
                }

                using(SqlCommand clickQuery = new SqlCommand("SELECT CLICKS FROM ADStatistics WHERE ID = @adID;", connection))
                {
                    clickQuery.Parameters.AddWithValue("@adID", adID);
                    using(SqlDataReader reader = await clickQuery.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        currentClicks = (int)reader["CLICKS"];
                    }
                }

                using (SqlCommand registerClickQuery = new SqlCommand("UPDATE ADStatistics SET CLICKS = @clicks WHERE ID = @adID;", connection))
                {
                    registerClickQuery.Parameters.AddWithValue("@adID", adID);
                    registerClickQuery.Parameters.AddWithValue("@clicks", currentClicks + 1);
                    await registerClickQuery.ExecuteNonQueryAsync();
                }
            }
            catch { }

            return Redirect(redirect);
        }
    }
}
