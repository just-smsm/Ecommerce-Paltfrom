using Ecommerce_platforms.Core.IRepository;
using Ecommerce_platforms.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Repository.Repository
{
     public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context,
                          IBrand brand,
                          ICart cart,
                          IDeliveryMethod deliveryMethod,
                          IImageService imageService,
                          IOrder order,
                          IProduct product)
        {
            _context = context;
            Brand = brand;
            Cart = cart;
            DeliveryMethod = deliveryMethod;
            ImageService = imageService;
            Order = order;
            Product = product;
        }

        public IBrand Brand { get; }
        public ICart Cart { get; }
        public IDeliveryMethod DeliveryMethod { get; }
        public IImageService ImageService { get; }
        public IOrder Order { get; }
        public IProduct Product { get; }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
