using BalanceChecker.Core.IConfiguration;
using BalanceChecker.Helper;
using BalanceChecker.Model;
using BalanceChecker.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BalanceChecker.Helper.Enum;

namespace BalanceChecker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private static Random random = new Random();

        public PaymentController(ILogger<PaymentController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// return records base on the account Number
        /// </summary>
        /// <param name="AccountNumber">Required parameter</param>
        /// <returns>return list of data base on the account number using json format</returns>
        [HttpGet("{AccountNumber}")]
        [ActionName("GetAccountNumber")]
        public async Task<IActionResult> GetAccountNumber(string AccountNumber)
        {
            var data = await _unitOfWork.Payments.All(AccountNumber);

          // view only the needdded columns in json string
          var newdata = data.Select(a => new VMPayment
            {
                Account_Balance = a.AccountBalance,
                Date = a.Date.ToString("ddd, dd MMM yyy hh:mm tt"),
                Amount = a.Amount,
                Status = a.Status == 0 ? "Closed" : "Open",
                Remarks = a.Status == 0 ? a.Remarks : ""
            }).ToList();

            var jsonFile = JsonConvert.SerializeObject(newdata);

            return Ok(jsonFile);
        }

        //Execute this function to create data in database
        [HttpPost]
        public async Task<IActionResult> CreatePayment(Payment payment)
        {
            if (ModelState.IsValid)
            {
                //Check if AccountNumber is exists
                var data = _unitOfWork.Payments.Find(x => x.AccountNumber == payment.AccountNumber).Result.OrderByDescending(x => x.Date).Take(1).FirstOrDefault();

                payment.Status = (int)Status.Open;

                if (data != null)
                {
                    //Less amount in Accountbalance
                    payment.AccountBalance = data.AccountBalance - payment.Amount;

                    //Change Status
                    if(payment.AccountBalance <= 0) payment.Status = (int)Status.Closed;


                    await _unitOfWork.Payments.Add(payment);
                    await _unitOfWork.CompleteAsync();
                }
                else
                {
                    payment.AccountNumber = ExtensionHelper.GenerateRandom(6);

                    await _unitOfWork.Payments.Add(payment);
                    await _unitOfWork.CompleteAsync();
                }
                return CreatedAtAction("GetItem", new { payment.TransactionId }, payment);
            }

            return new JsonResult("Something Went wrong") { StatusCode = 500 };
        }

        #region Uncomment this part if you want to use the function
        /*
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _unitOfWork.Payments.All();
            return Ok(data);
        } 
         
        [HttpPut("{TransactionId}")]
        public async Task<IActionResult> UpdateItem(int TransactionId, Payment payment)
        {
            if (TransactionId != payment.TransactionId)
                return BadRequest();

            await _unitOfWork.Payments.Upsert(payment);
            await _unitOfWork.CompleteAsync();

            // Following up the REST standart on update we need to return NoContent
            return NoContent();
        }

        [HttpDelete("{TransactionId}")]
        public async Task<IActionResult> DeleteItem(int TransactionId)
        {
            var item = await _unitOfWork.Payments.GetById(TransactionId);

            if (item == null)
                return BadRequest();

            await _unitOfWork.Payments.Delete(TransactionId);
            await _unitOfWork.CompleteAsync();

            return Ok(item);
        }

        [HttpGet("{TransactionId}")]
        public async Task<IActionResult> GetItem(int TransactionId)
        {
            var item = await _unitOfWork.Payments.GetById(TransactionId);

            if (item == null)
                return NotFound();

            return Ok(item);
        }
        */
        #endregion

    }
}
