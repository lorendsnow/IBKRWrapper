using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;
using IBKRWrapper.Utils;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private readonly object accountValuesLock = new();

        public Task<List<PortfolioPosition>> GetPortfolioPositionsAsync(string account)
        {
            lock (accountValuesLock)
            {
                List<PortfolioPosition> positions = [];
                TaskCompletionSource<List<PortfolioPosition>> tcs = new();

                EventHandler<UpdatePortfolioEventArgs> updatePortfolioHandler =
                    HandlerFactory.MakeUpdatePortfolioHandler(positions);
                EventHandler<AccountDownloadEndEventArgs> accountDownloadEndHandler =
                    HandlerFactory.MakeAccountDownloadEndHandler(account, tcs, positions);

                UpdatePortfolioEvent += updatePortfolioHandler;
                AccountDownloadEndEvent += accountDownloadEndHandler;

                tcs.Task.ContinueWith(t =>
                {
                    UpdatePortfolioEvent -= updatePortfolioHandler;
                    AccountDownloadEndEvent -= accountDownloadEndHandler;
                });

                clientSocket.reqAccountUpdates(true, account);

                return tcs.Task;
            }
        }

        public Task<Dictionary<string, string>> GetAccountValuesAsync(string account)
        {
            TaskCompletionSource<Dictionary<string, string>> tcs = new();
            lock (accountValuesLock)
            {
                Dictionary<string, string> accountValues = [];

                EventHandler<UpdateAccountValueEventArgs> updateAccountValueHandler =
                    HandlerFactory.MakeUpdateAccountValueHandler(account, accountValues);
                EventHandler<AccountDownloadEndEventArgs> accountDownloadEndHandler =
                    HandlerFactory.MakeAccountDownloadEndHandler(account, tcs, accountValues);

                UpdateAccountValueEvent += updateAccountValueHandler;
                AccountDownloadEndEvent += accountDownloadEndHandler;

                tcs.Task.ContinueWith(t =>
                {
                    UpdateAccountValueEvent -= updateAccountValueHandler;
                    AccountDownloadEndEvent -= accountDownloadEndHandler;
                });

                clientSocket.reqAccountUpdates(true, account);
            }
            return tcs.Task;
        }

        public event EventHandler<UpdatePortfolioEventArgs>? UpdatePortfolioEvent;

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
            UpdatePortfolioEvent?.Invoke(
                this,
                new UpdatePortfolioEventArgs(
                    new PortfolioPosition(
                        contract,
                        position,
                        marketPrice,
                        marketValue,
                        averageCost,
                        unrealizedPNL,
                        realizedPNL,
                        accountName
                    )
                )
            );
        }

        public event EventHandler<UpdateAccountValueEventArgs>? UpdateAccountValueEvent;

        public void updateAccountValue(
            string key,
            string value,
            string currency,
            string accountName
        )
        {
            UpdateAccountValueEvent?.Invoke(
                this,
                new UpdateAccountValueEventArgs(key, value, currency, accountName)
            );
        }

        public event EventHandler<AccountDownloadEndEventArgs>? AccountDownloadEndEvent;

        public void accountDownloadEnd(string account)
        {
            AccountDownloadEndEvent?.Invoke(this, new AccountDownloadEndEventArgs(account));
            clientSocket.reqAccountUpdates(false, account);
        }

        public void updateAccountTime(string timestamp)
        {
            return;
        }
    }
}
