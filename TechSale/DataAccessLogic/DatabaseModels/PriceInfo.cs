using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLogic.DatabaseModels
{
    [Owned]
    public class PriceInfo
    {
        [Required(ErrorMessage = "Укажите стартовую цену")]
        public decimal StartPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        [Required(ErrorMessage = "Укажите шаг ставки")]
        public decimal BidStep { get; set; }
    }
}
