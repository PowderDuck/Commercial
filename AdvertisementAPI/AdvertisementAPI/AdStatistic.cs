using System.ComponentModel.DataAnnotations.Schema;

namespace AdvertisementAPI
{
    public class AdStatistic
    {
        //public int? STATID { get; set; }
        public int? STATID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ID { get; set; }
        public int VIEWS { get; set; }
        public int CLICKS { get; set; }
        public DateTime VALIDDATE { get; set; }
    }
}
