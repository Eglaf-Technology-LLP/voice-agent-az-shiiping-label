using Microsoft.Extensions.Logging;
using Order.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Services
{
    public interface ISalesOrderRepository
    {
        Task<bool> GenerateShippingLabel(ILogger log);
    }
}
