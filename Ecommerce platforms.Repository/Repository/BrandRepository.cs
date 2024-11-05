
using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Core.Models;
using Ecommerce_platforms.Repository.Data;
using ElSory.Repository.Data;


namespace Ecommerce_platforms.Repository.Repository
{
    public class BrandRepository : GenericRepository<Brand>, IBrand
    {
        public BrandRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implement additional methods if any
    }
}
