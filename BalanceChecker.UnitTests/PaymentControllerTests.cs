using BalanceChecker.Controllers;
using BalanceChecker.Core.IConfiguration;
using BalanceChecker.Helper;
using BalanceChecker.Model;
using BalanceChecker.ViewModel;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BalanceChecker.UnitTests
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task GetAccountNumber_WithUnexistingAccountNumber_ReturnNotFound()
        {
            var vmpaymentList = new List<VMPayment>();
            var vmpayment = new VMPayment();

            vmpayment.Amount = Faker.RandomNumber.Next(0, 10000);
            vmpayment.Date = DateTime.Now.ToLongDateString();
            vmpayment.Remarks = Faker.Lorem.Sentence(5);
            vmpayment.Status = ExtensionHelper.GenerateRandom(6);

            vmpaymentList.Add(vmpayment);

            var jsonFile = JsonConvert.SerializeObject(vmpaymentList);

            ////Arrange
            var unitOfWorkStub = new Mock<IUnitOfWork>();
            unitOfWorkStub.Setup(uow => uow.Payments.All(ExtensionHelper.GenerateRandom(6))).ToString();

            var loggerStub = new Mock<ILogger<PaymentController>>();

            var controller = new PaymentController(loggerStub.Object, unitOfWorkStub.Object);
            ////Act
            ///
             #region Real
            var resultRealValue = await controller.GetAccountNumber("QW7A2V");
            var UnBoxTheResultReal = (OkObjectResult)resultRealValue; // <-- Cast is before using it.
            var realValueReal = UnBoxTheResultReal.Value;

            #endregion
            #region Fake
            var resultFakeValue = await controller.GetAccountNumber(ExtensionHelper.GenerateRandom(6));
            var UnBoxTheResultFake = (OkObjectResult)resultFakeValue; // <-- Cast is before using it.
            var realValueFake = UnBoxTheResultFake.Value;

            #endregion
            ////Assert
            //  Assert.Equal(realValue.ToString(), jsonFile.ToString());



            Assert.Null((string)realValueFake);
            Assert.DoesNotContain("", jsonFile);
            Assert.Contains("[]", jsonFile);

            //Assert.NotNull((string)realValueFake);
            //Assert.NotSame("", jsonFile);
            //Assert.Contains("[]", jsonFile);




        }
    }
}
