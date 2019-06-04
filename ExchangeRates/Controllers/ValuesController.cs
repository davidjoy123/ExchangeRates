using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRates.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExchangeRates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        //[AcceptVerbs("SEARCH")]
        [HttpPost("/search")]
        public async Task<IActionResult> Search([FromBody] ExchangeSearchModel searchModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //..perform search
                HttpClient _httpClient = new HttpClient();

                //Remove duplicates.
                searchModel.Period = searchModel.Period.Distinct().ToArray();
                string startDate = searchModel.Period.Min(record => record.Date).ToString("yyyy-MM-dd");
                string endDate = searchModel.Period.Max(record => record.Date).ToString("yyyy-MM-dd");

                var response = await _httpClient.GetAsync("https://api.exchangeratesapi.io/history?start_at=" + startDate + "&end_at=" + endDate + "&symbols=" + searchModel.TargetCurrency + "&base=" + searchModel.BaseCurrency);
                var resultContent = await Task.Run(() => response.Content.ReadAsStringAsync());

                JObject parent = JObject.Parse(resultContent);
                var rates = parent.Value<JObject>("rates").Properties();

                var ratesDict = rates
                                .ToDictionary(
                                k => k.Name,
                                v => v.Value.SelectToken(searchModel.TargetCurrency).ToString());

                HistoricalExchangeRates historicalExchangeRates = new HistoricalExchangeRates();
                historicalExchangeRates.MinRate = 0;
                historicalExchangeRates.MaxRate = 0;
                historicalExchangeRates.AverageRate = 0;
                historicalExchangeRates.BaseCurrency = searchModel.BaseCurrency;
                historicalExchangeRates.TargetCurrency = searchModel.TargetCurrency;

                double hits = 0;
                double totalValue = 0;

                for (int i = 0; i < searchModel.Period.Length; i++ )
                {
                    //Search date in the rates Dictionary
                    string value = "";
                    string searchDate = searchModel.Period[i].ToString("yyyy-MM-dd");
                    if (ratesDict.TryGetValue(searchDate, out value))
                    {
                        double currentValue = Double.Parse(value);
                        hits = hits + 1;
                        totalValue = totalValue + currentValue;

                        //Set initial values.
                        if (historicalExchangeRates.MinRate == 0)
                            historicalExchangeRates.MinRate = currentValue;
                        if (historicalExchangeRates.MaxRate == 0)
                            historicalExchangeRates.MaxRate = currentValue;

                        if (currentValue <= historicalExchangeRates.MinRate)
                        {
                            historicalExchangeRates.MinRate = currentValue;
                            historicalExchangeRates.DateOfMinRate = searchDate;
                        }
                        else if(currentValue >= historicalExchangeRates.MaxRate)
                        {
                            historicalExchangeRates.MaxRate = currentValue;
                            historicalExchangeRates.DateOfMaxRate = searchDate;
                        }
                    } 
                }

                //Average value.
                if (hits > 0)
                    historicalExchangeRates.AverageRate = totalValue / hits;

                return Ok(historicalExchangeRates);
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}
