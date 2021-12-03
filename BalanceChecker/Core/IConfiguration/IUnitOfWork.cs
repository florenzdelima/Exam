using BalanceChecker.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceChecker.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        IPaymentRepository Payments { get; }

        //responsible for sending the changes back to the database
        Task CompleteAsync();
    }
}
