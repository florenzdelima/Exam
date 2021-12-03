using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceChecker.Model
{
    public class Payment
    {
        //assuming this is a FK
        public string AccountNumber { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public double AccountBalance { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
    }
}
