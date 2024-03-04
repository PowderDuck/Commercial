using Microsoft.EntityFrameworkCore;

namespace AdvertisementAPI
{
    [PrimaryKey("Username")]
    public class Client
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
