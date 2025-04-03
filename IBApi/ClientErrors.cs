namespace IBApi
{
    public class ClientErrors
    {
        public static readonly CodeMsgPair AlreadyConnected = new(501, "Already Connected.");
        public static readonly CodeMsgPair CONNECT_FAIL = new(
            502,
            @"Couldn't connect to TWS. Confirm that ""Enable ActiveX and Socket Clients""
                            is enabled and connection port is the same as ""Socket Port"" on the TWS ""Edit->Global Configuration...->API->Settings"" menu.
                            Live Trading ports: TWS: 7496; IB Gateway: 4001. Simulated Trading ports for new installations of version 954.1 or newer:
                            TWS: 7497; IB Gateway: 4002"
        );
        public static readonly CodeMsgPair UPDATE_TWS = new(
            503,
            "The TWS is out of date and must be upgraded."
        );
        public static readonly CodeMsgPair NOT_CONNECTED = new(504, "Not connected");
        public static readonly CodeMsgPair UNKNOWN_ID = new(
            505,
            "Fatal Error: Unknown message id."
        );
        public static readonly CodeMsgPair FAIL_SEND_REQMKT = new(
            510,
            "Request Market Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANMKT = new(
            511,
            "Cancel Market Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_ORDER = new(512, "Order Sending Error - ");
        public static readonly CodeMsgPair FAIL_SEND_ACCT = new(
            513,
            "Account Update Request Sending Error -"
        );
        public static readonly CodeMsgPair FAIL_SEND_EXEC = new(
            514,
            "Request For Executions Sending Error -"
        );
        public static readonly CodeMsgPair FAIL_SEND_CORDER = new(
            515,
            "Cancel Order Sending Error -"
        );
        public static readonly CodeMsgPair FAIL_SEND_OORDER = new(
            516,
            "Request Open Order Sending Error -"
        );
        public static readonly CodeMsgPair UNKNOWN_CONTRACT = new(
            517,
            "Unknown contract. Verify the contract details supplied."
        );
        public static readonly CodeMsgPair FAIL_SEND_REQCONTRACT = new(
            518,
            "Request Contract Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQMKTDEPTH = new(
            519,
            "Request Market Depth Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANMKTDEPTH = new(
            520,
            "Cancel Market Depth Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_SERVER_LOG_LEVEL = new(
            521,
            "Set Server Log Level Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_FA_REQUEST = new(
            522,
            "FA Information Request Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_FA_REPLACE = new(
            523,
            "FA Information Replace Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQSCANNER = new(
            524,
            "Request Scanner Subscription Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANSCANNER = new(
            525,
            "Cancel Scanner Subscription Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQSCANNERPARAMETERS = new(
            526,
            "Request Scanner Parameter Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQHISTDATA = new(
            527,
            "Request Historical Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANHISTDATA = new(
            528,
            "Request Historical Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQRTBARS = new(
            529,
            "Request Real-time Bar Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANRTBARS = new(
            530,
            "Cancel Real-time Bar Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQCURRTIME = new(
            531,
            "Request Current Time Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQFUNDDATA = new(
            532,
            "Request Fundamental Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANFUNDDATA = new(
            533,
            "Cancel Fundamental Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQCALCIMPLIEDVOLAT = new(
            534,
            "Request Calculate Implied Volatility Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQCALCOPTIONPRICE = new(
            535,
            "Request Calculate Option Price Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCALCIMPLIEDVOLAT = new(
            536,
            "Cancel Calculate Implied Volatility Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCALCOPTIONPRICE = new(
            537,
            "Cancel Calculate Option Price Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQGLOBALCANCEL = new(
            538,
            "Request Global Cancel Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQMARKETDATATYPE = new(
            539,
            "Request Market Data Type Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQPOSITIONS = new(
            540,
            "Request Positions Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANPOSITIONS = new(
            541,
            "Cancel Positions Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQACCOUNTDATA = new(
            542,
            "Request Account Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANACCOUNTDATA = new(
            543,
            "Cancel Account Data Sending Error - "
        );

        public static readonly CodeMsgPair FAIL_SEND_VERIFYREQUEST = new(
            544,
            "Verify Request Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_VERIFYMESSAGE = new(
            545,
            "Verify Message Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_QUERYDISPLAYGROUPS = new(
            546,
            "Query Display Groups Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_SUBSCRIBETOGROUPEVENTS = new(
            547,
            "Subscribe To Group Events Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_UPDATEDISPLAYGROUP = new(
            548,
            "Update Display Group Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_UNSUBSCRIBEFROMGROUPEVENTS = new(
            549,
            "Unsubscribe From Group Events Sending Error - "
        );
        public static readonly CodeMsgPair BAD_LENGTH = new(507, "Bad message length");
        public static readonly CodeMsgPair BAD_MESSAGE = new(508, "Bad message");
        public static readonly CodeMsgPair UNSUPPORTED_VERSION = new(506, "Unsupported version");

        public static readonly CodeMsgPair FAIL_SEND_VERIFYANDAUTHREQUEST = new(
            551,
            "Verify And Auth Request Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_VERIFYANDAUTHMESSAGE = new(
            552,
            "Verify And Auth Message Sending Error - "
        );

        public static readonly CodeMsgPair FAIL_SEND_REQPOSITIONSMULTI = new(
            553,
            "Request Positions Multi Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANPOSITIONSMULTI = new(
            554,
            "Cancel Positions Multi Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQACCOUNTUPDATESMULTI = new(
            555,
            "Request Account Updates Multi Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANACCOUNTUPDATESMULTI = new(
            556,
            "Cancel Account Updates Multi Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQSECDEFOPTPARAMS = new(
            557,
            "Request Security Definition Option Parameters Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQSOFTDOLLARTIERS = new(
            558,
            "Request Soft Dollar Tiers Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQFAMILYCODES = new(
            559,
            "Request Family Codes Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQMATCHINGSYMBOLS = new(
            560,
            "Request Matching Symbols Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQMKTDEPTHEXCHANGES = new(
            561,
            "Request Market Depth Exchanges Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQSMARTCOMPONENTS = new(
            562,
            "Request Smart Components Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQNEWSPROVIDERS = new(
            563,
            "Request News Providers Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQNEWSARTICLE = new(
            564,
            "Request News Article Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQHISTORICALNEWS = new(
            565,
            "Request Historical News Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQHEADTIMESTAMP = new(
            566,
            "Request Head Time Stamp Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQHISTOGRAMDATA = new(
            567,
            "Request Histogram Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCELHISTOGRAMDATA = new(
            568,
            "Cancel Request Histogram Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCELHEADTIMESTAMP = new(
            569,
            "Cancel Head Time Stamp Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQMARKETRULE = new(
            570,
            "Request Market Rule Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQPNL = new(
            571,
            "Request PnL Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCELPNL = new(
            572,
            "Cancel PnL Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQPNLSINGLE = new(
            573,
            "Request PnL Single Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCELPNLSINGLE = new(
            574,
            "Cancel PnL Single Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQHISTORICALTICKS = new(
            575,
            "Request Historical Ticks Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQTICKBYTICKDATA = new(
            576,
            "Request Tick-By-Tick Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CANCELTICKBYTICKDATA = new(
            577,
            "Cancel Tick-By-Tick Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQCOMPLETEDORDERS = new(
            578,
            "Request Completed Orders Sending Error - "
        );
        public static readonly CodeMsgPair INVALID_SYMBOL = new(579, "Invalid symbol in string - ");
        public static readonly CodeMsgPair FAIL_SEND_REQ_WSH_META_DATA = new(
            580,
            "Request WSH Meta Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CAN_WSH_META_DATA = new(
            581,
            "Cancel WSH Meta Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQ_WSH_EVENT_DATA = new(
            582,
            "Request WSH Event Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_CAN_WSH_EVENT_DATA = new(
            583,
            "Cancel WSH Event Data Sending Error - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQ_USER_INFO = new(
            584,
            "Request User Info Sending Error - "
        );
        public static readonly CodeMsgPair FA_PROFILE_NOT_SUPPORTED = new(
            585,
            "FA Profile is not supported anymore, use FA Group instead - "
        );
        public static readonly CodeMsgPair FAIL_SEND_REQCURRTIMEINMILLIS = new(
            586,
            "Request Current Time In Millis Sending Error - "
        );

        public static readonly CodeMsgPair FAIL_GENERIC = new(
            -1,
            "Specific error message needs to be given for these requests! "
        );
    }

    public class CodeMsgPair(int code, string message)
    {
        public int Code { get; } = code;

        public string Message { get; } = message;
    }
}
