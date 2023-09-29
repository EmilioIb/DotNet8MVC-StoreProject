using StoreProject.DataAccess.Data;
using StoreProject.DataAccess.Repository.IRepository;
using StoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreProject.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository 
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderHeaderID == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymendIntentId)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderHeaderID == id);
            if(!string.IsNullOrEmpty(sessionId)) {
                orderFromDb.SessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymendIntentId))
            {
                orderFromDb.PaymentIntentId = paymendIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }

        }
    }
}
