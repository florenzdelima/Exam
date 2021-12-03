using System;

namespace BalanceChecker.ViewModel
{
    public class VMPayment
    {
        public double Account_Balance { get; set; }
        public double Amount { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
