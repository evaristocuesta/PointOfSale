using System.ComponentModel.DataAnnotations;

namespace PointOfSale.WebAPI.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
