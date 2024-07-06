using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<ContractDetailsEventArgs>? ContractDetailsEvent;

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            ContractDetailsEvent?.Invoke(
                this,
                new ContractDetailsEventArgs(reqId, contractDetails)
            );
        }

        public event EventHandler<ContractDetailsEndEventArgs>? ContractDetailsEndEvent;

        public void contractDetailsEnd(int reqId)
        {
            ContractDetailsEndEvent?.Invoke(this, new ContractDetailsEndEventArgs(reqId));
        }

        public event EventHandler<OptionsChainEventArgs>? OptionsChainEvent;

        public void securityDefinitionOptionParameter(
            int reqId,
            string exchange,
            int underlyingConId,
            string tradingClass,
            string multiplier,
            HashSet<string> expirations,
            HashSet<double> strikes
        )
        {
            OptionsChainEvent?.Invoke(
                this,
                new OptionsChainEventArgs(
                    reqId,
                    exchange,
                    underlyingConId,
                    tradingClass,
                    multiplier,
                    expirations,
                    strikes
                )
            );
        }

        public event EventHandler<OptionsChainEndEventArgs>? OptionsChainEndEvent;

        public void securityDefinitionOptionParameterEnd(int reqId)
        {
            OptionsChainEndEvent?.Invoke(this, new OptionsChainEndEventArgs(reqId));
        }
    }
}
