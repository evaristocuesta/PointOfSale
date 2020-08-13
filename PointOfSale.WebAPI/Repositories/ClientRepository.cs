using PointOfSale.WebAPI.Data;
using PointOfSale.WebAPI.Models;

namespace PointOfSale.WebAPI.Repositories
{
    public class ClientRepository : GenericRepository<Client, PointOfSaleDbContext>, IClientRepository
    {
        public ClientRepository(PointOfSaleDbContext context) : base(context)
        {

        }
    }
}
