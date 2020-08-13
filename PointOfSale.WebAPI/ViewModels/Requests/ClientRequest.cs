using System.ComponentModel.DataAnnotations;

namespace PointOfSale.WebAPI.ViewModels.Requests
{
    public class ClientRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
