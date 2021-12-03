using BalanceChecker.Core.IRepositories;
using BalanceChecker.Core.Repositories;
using BalanceChecker.Data;
using BalanceChecker.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceChecker.Core.Repositories
{

    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context, ILogger logger) : base(context, logger) { }

        public override async Task<IEnumerable<Payment>> All(string accountNumber)
        {
            try
            {
                var data = await dbSet.Where(x => x.AccountNumber == accountNumber).OrderByDescending(x => x.Date).ToListAsync();

                return data.Select(a => new Payment
                {
                    AccountBalance = data.Take(1).FirstOrDefault().AccountBalance, // Get the latest Account Balance
                    Date = a.Date,
                    Amount = a.Amount,
                    Status = a.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(PaymentRepository));
                return new List<Payment>();
            }
        }

        public override async Task<IEnumerable<Payment>> All()
        {
            try
            {
                return await dbSet.OrderByDescending(x => x.Date).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(PaymentRepository));
                return new List<Payment>();
            }
        }

        //Upsert method, if data is not found then goto insert condition else update the data
        public override async Task<bool> Upsert(Payment entity)
        {
            try
            {
                var data = await dbSet.Where(x => x.TransactionId == entity.TransactionId).FirstOrDefaultAsync();

                if (data == null)
                    return await Add(entity);

                data.AccountBalance = data.AccountBalance - entity.Amount;
                data.Amount = entity.Amount;
                data.Remarks = entity.Remarks;
                data.Status = entity.Status;
                data.Date = DateTime.UtcNow;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert function error", typeof(PaymentRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(int id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.TransactionId == id).FirstOrDefaultAsync();

                if (exist == null) return false;

                dbSet.Remove(exist);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(PaymentRepository));
                return false;
            }
        }

        //public override async Task<T> GetByAccountNumber(string accountNumber)
        //{
        //    try
        //    {
        //        return await dbSet.Where(x => x.AccountNumber == accountNumber).OrderBy(x => x.Date).Take(1).FirstOrDefaultAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "{Repo} Delete function error", typeof(PaymentRepository));
        //        return false;
        //    }
        //}
    }
}