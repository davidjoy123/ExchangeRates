using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRates.Models
{
    public class HistoricalExchangeRates
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public double MinRate { get; set; }
        public string DateOfMinRate { get; set; }
        public double MaxRate { get; set; }
        public string DateOfMaxRate { get; set; }
        public double AverageRate { get; set; }
    }
}
