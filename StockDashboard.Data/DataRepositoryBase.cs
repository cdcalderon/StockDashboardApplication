using StockDashboard.Core;
using StockDashboard.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StockDashboard.Data
{
    public abstract class DataRepositoryBase<T> : DataRepositoryBase<T, StockDashboardContext>
        where T : class, IIdentifiableEntity, new()
    {
    }
}
