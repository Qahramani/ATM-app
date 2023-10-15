using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.Domain.Entities
{
    public class InternalTransfer
    {
        public decimal TransferAmount { get; set; }
        public long ReciepentBankAccountNumber { get; set; }
        public string ReciepentBankAccountName { get; set; }
    }
}
