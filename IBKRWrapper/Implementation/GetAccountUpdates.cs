using System.Collections.Concurrent;
using IBApi;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private ConcurrentBag<PortfolioPosition>? _portfolioPositionsResults = null;
        private Dictionary<
            string,
            TaskCompletionSource<ConcurrentBag<PortfolioPosition>>
        > _portfolioPositionsTcs = [];
        private Dictionary<string, string> _accountValues = [];
        private Dictionary<
            string,
            TaskCompletionSource<Dictionary<string, string>>
        > _accountValuesTcs = [];

        /// <summary>
        /// Gets account updates from the TWS server.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<ConcurrentBag<PortfolioPosition>> GetPortfolioPositionsAsync(string account)
        {
            _portfolioPositionsResults = [];
            TaskCompletionSource<ConcurrentBag<PortfolioPosition>> tcs = new();
            _portfolioPositionsTcs.Add(account, tcs);

            clientSocket.reqAccountUpdates(false, account);

            return tcs.Task;
        }

        /// <summary>
        /// Return the dictionary of account values from the TWS server.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<Dictionary<string, string>> GetAccountValuesAsync(string account)
        {
            TaskCompletionSource<Dictionary<string, string>> tcs = new();
            _accountValuesTcs.Add(account, tcs);

            clientSocket.reqAccountUpdates(true, account);

            return tcs.Task;
        }

        public void updatePortfolio(
            Contract contract,
            decimal position,
            double marketPrice,
            double marketValue,
            double averageCost,
            double unrealizedPNL,
            double realizedPNL,
            string accountName
        )
        {
            if (_portfolioPositionsResults == null)
            {
                return;
            }
            _portfolioPositionsResults.Add(
                PositionBuilder.Build(
                    contract,
                    position,
                    marketPrice,
                    marketValue,
                    averageCost,
                    unrealizedPNL,
                    realizedPNL,
                    accountName
                )
            );
        }

        public void updateAccountValue(
            string key,
            string value,
            string currency,
            string accountName
        ) => _accountValues[key] = value;

        public void accountDownloadEnd(string account)
        {
            if (
                _portfolioPositionsTcs.ContainsKey(account)
                && _portfolioPositionsResults is not null
            )
            {
                _portfolioPositionsTcs[account].SetResult(_portfolioPositionsResults);
                _portfolioPositionsResults = null;
                _portfolioPositionsTcs.Remove(account);
            }
            if (_accountValuesTcs.ContainsKey(account))
            {
                _accountValuesTcs[account].SetResult(_accountValues);
                _accountValues = [];
                _accountValuesTcs.Remove(account);
            }
        }

        public void updateAccountTime(string timestamp)
        {
            return;
        }
    }
}
