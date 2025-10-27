/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using NodaTime;
using QuantConnect.Interfaces;
using QuantConnect.Securities;

namespace QuantConnect.Scheduling
{
    /// <summary>
    /// Base rule scheduler
    /// </summary>
    public class BaseScheduleRules
    {
        private bool _sentImplicitWarning;
        private readonly IAlgorithm _algorithm;

        /// <summary>
        /// The algorithm's default time zone
        /// </summary>
        protected DateTimeZone TimeZone { get; set; }

        /// <summary>
        /// The security manager
        /// </summary>
        protected SecurityManager Securities { get; set; }

        /// <summary>
        /// The market hours database instance to use
        /// </summary>
        protected MarketHoursDatabase MarketHoursDatabase { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRules"/> helper class
        /// </summary>
        /// <param name="algorithm">The algorithm instance</param>
        /// <param name="securities">The security manager</param>
        /// <param name="timeZone">The algorithm's default time zone</param>
        /// <param name="marketHoursDatabase">The market hours database instance to use</param>
        public BaseScheduleRules(IAlgorithm algorithm, SecurityManager securities, DateTimeZone timeZone, MarketHoursDatabase marketHoursDatabase)
        {
            TimeZone = timeZone;
            _algorithm = algorithm;
            Securities = securities;
            MarketHoursDatabase = marketHoursDatabase;
        }

        /// <summary>
        /// Helper method to fetch the security exchange hours
        /// </summary>
        protected SecurityExchangeHours GetSecurityExchangeHours(Symbol symbol)
        {
            if (!Securities.TryGetValue(symbol, out var security))
            {
                return MarketHoursDatabase.GetEntry(symbol.ID.Market, symbol, symbol.SecurityType).ExchangeHours;
            }
            return security.Exchange.Hours;
        }

        protected Symbol GetSymbol(string ticker)
        {
            if (SymbolCache.TryGetSymbol(ticker, out var symbolCache))
            {
                return symbolCache;
            }

            if (!_sentImplicitWarning)
            {
                _sentImplicitWarning = true;
                _algorithm?.Debug($"Warning: no existing symbol found for ticker {ticker}, it will be created with {SecurityType.Equity} type.");
            }
            symbolCache = Symbol.Create(ticker, SecurityType.Equity, Market.USA);
            SymbolCache.Set(ticker, symbolCache);
            return symbolCache;
        }
    }
}
