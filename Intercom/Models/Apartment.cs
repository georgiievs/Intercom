using System.ComponentModel.DataAnnotations;

namespace Intercom.Models
{
    public class Apartment
    {
        [Key]
        public int Code { get; set; }
        public bool IsBlocked { get; set; }
    }
}
