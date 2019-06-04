using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRates.Models
{
    public class ExchangeSearchModel
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public DateTime[] Period { get; set; }
    }
}
