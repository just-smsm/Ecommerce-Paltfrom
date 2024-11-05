using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Core.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IBrand Brand { get; }
        ICart Cart { get; }
        IDeliveryMethod DeliveryMethod { get; }
        IImageService ImageService { get; }
        IOrder Order { get; }
        IProduct Product { get; }
        Task<int> Complete();
    }
}
