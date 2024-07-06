using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
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
