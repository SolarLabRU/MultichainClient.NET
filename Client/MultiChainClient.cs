using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Platform.DataAccess.MultiChain.Model;

namespace Platform.DataAccess.MultiChain.Client
{
    /// <summary>
    /// JSON-RPC client
    /// </summary>
    public class MultiChainClient
    {
        private string Hostname { get; set; }
        private int Port { get; set; }
        private bool UseSsl { get; set; }
        private string ChainName { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        //public event EventHandler<EventArgs<JsonRpcRequest>> Executing;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="useSsl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="chainName"></param>
        public MultiChainClient(string hostname, int port, bool useSsl, string username, string password,
            string chainName)
        {
            Hostname = hostname;
            Port = port;
            UseSsl = useSsl;
            Username = username;
            Password = password;
            ChainName = chainName;
        }

        #region -= General utilities =-

        /// <summary>
        /// Returns a list of values of this blockchain’s parameters, which are fixed by the chain’s genesis block and the same for all nodes.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<Dictionary<string, object>>> GetBlockchainParamsAsync()
        {
            return ExecuteAsync<Dictionary<string, object>>("getblockchainparams", 0);
        }

        /// <summary>
        /// Returns general information about this node and blockchain. MultiChain adds some fields to Bitcoin Core’s response, 
        /// giving the blockchain’s chainname, description, protocol, peer-to-peer port. There are also incomingpaused and miningpaused fields – see the pause command. 
        /// The burnaddress is an address with no known private key, to which assets can be sent to make them provably unspendable. 
        /// The nodeaddress can be passed to other nodes for connecting. 
        /// The setupblocks field gives the length in blocks of the setup phase in which some consensus constraints are not applied.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<GetInfoResponse>> GetInfoAsync()
        {
            return ExecuteAsync<GetInfoResponse>("getinfo", 1);
        }

        /// <summary>
        /// Returns a list of available API commands, including MultiChain-specific commands.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> HelpAsync()
        {
            return ExecuteAsync<string>("help", 0);
        }

        /// <summary>
        /// Shuts down the this blockchain node, i.e. stops the multichaind process.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<object>> StopAsync()
        {
            return ExecuteAsync<object>("stop", 0);
        }

        #endregion

        #region -= Managing wallet addresses =-

        /// <summary>
        /// Creates a pay-to-scripthash (P2SH) multisig address and adds it to the wallet. 
        /// Funds sent to this address can only be spent by transactions signed by nrequired of the specified keys. 
        /// Each key can be a full public key, or an address if the corresponding key is in the node’s wallet. 
        /// (Public keys for a wallet’s addresses can be obtained using the getaddresses call with verbose=true.)
        /// </summary>
        /// <param name="numRequired"></param>
        /// <param name="addresses"></param>
        /// <param name="account"></param>
        /// <returns>Returns the P2SH address.</returns>
        public Task<JsonRpcResponse<string>> AddMultiSigAddressAsync(int numRequired, IEnumerable<string> addresses,
            string account = null)
        {
            return ExecuteAsync<string>("addmultisigaddress", 0, numRequired, addresses, account ?? string.Empty);
        }

        /// <summary>
        /// Returns a list of addresses in this node’s wallet
        /// </summary>
        /// <param name="verbose">Set verbose to true to get more information about each address, formatted like the output of the validateaddress command</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<string>>> GetAddressesAsync(bool verbose = false)
        {
            return ExecuteAsync<List<string>>("getaddresses", 0, verbose);
        }

        /// <summary>
        /// Returns a new address whose private key is added to the wallet.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> GetNewAddressAsync()
        {
            return ExecuteAsync<string>("getnewaddress", 0);
        }

        /// <summary>
        /// This creates one or more watch-only addresses, 
        /// whose activity and balance can be retrieved via various APIs (e.g. with the includeWatchOnly parameter), but whose funds cannot be spent by this node
        /// </summary>
        /// <param name="address">Address or a full public key, or an array of either</param>
        /// <param name="label"></param>
        /// <param name="rescan">If rescan is true, the entire blockchain is checked for transactions relating to all addresses in the wallet, including the added ones</param>
        /// <returns>Returns null if successful</returns>
        public Task<JsonRpcResponse<string>> ImportAddressAsync(string address, string label = null, bool rescan = true)
        {
            return ExecuteAsync<string>("importaddress", 0, address, label ?? string.Empty, rescan);
        }

        /// <summary>
        /// Returns information about the addresses in the wallet. 
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<AddressResponse>>> ListAddressesAsync()
        {
            return ExecuteAsync<List<AddressResponse>>("listaddresses", 0);
        }

        #endregion

        #region -= Working with non-wallet addresses =-

        /// <summary>
        /// Creates a pay-to-scripthash (P2SH) multisig address.
        /// </summary>
        /// <param name="numRequired">Funds sent to this address can only be spent by transactions signed by nrequired of the specified keys.</param>
        /// <param name="addresses">Each address can be a full hexadecimal public key, or an address if the corresponding key is in the node’s wallet</param>
        /// <returns>Returns an object containing the P2SH address and corresponding redeem script.</returns>
        public Task<JsonRpcResponse<MultiSigResponse>> CreateMultiSigAsync(int numRequired,
            IEnumerable<string> addresses)
        {
            return ExecuteAsync<MultiSigResponse>("createmultisig", 0, numRequired, addresses);
        }

        /// <summary>
        /// Returns information about address including a check for its validity.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<AddressResponse>> ValidateAddressAsync(string address)
        {
            return ExecuteAsync<AddressResponse>("validateaddress", 0, address);
        }

        #endregion

        #region -= Permissions management =-

        /// <summary>
        /// Grants permissions to addresses, a comma-separated list of addresses. 
        /// For global permissions, set permissions to one of connect, send, receive, create, issue, mine, activate, admin, or a comma-separated list thereof. 
        /// For per-asset or per-stream permissions, use the form entity.issue or entity.write,admin where entity is an asset or stream name, ref or creation txid. 
        /// If the chain uses a native currency, you can send some to each recipient using the native-amount parameter
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="permissions"></param>
        /// <param name="nativeAmount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <param name="startBlock"></param>
        /// <param name="endBlock"></param>
        /// <returns>Returns the txid of the transaction granting the permissions</returns>
        public Task<JsonRpcResponse<string>> GrantAsync(IEnumerable<string> addresses,
            BlockchainPermissions permissions, decimal nativeAmount = 0M, string comment = null,
            string commentTo = null, int startBlock = 0, int endBlock = 0)
        {
            var stringifiedAddresses = StringifyValues(addresses);
            var permissionsAsString = FormatPermissions(permissions);
            return ExecuteAsync<string>("grant", 0, stringifiedAddresses, permissionsAsString);
        }

        /// <summary>
        /// This works like grant, but with control over the from-address used to grant the permissions. 
        /// It is useful if the node has multiple addresses with administrator permissions.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddresses"></param>
        /// <param name="permissions"></param>
        /// <param name="nativeAmount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <param name="startBlock"></param>
        /// <param name="endBlock"></param>
        /// <returns></returns>
        /// <see cref="GrantAsync"/>
        public Task<JsonRpcResponse<string>> GrantFromAsync(string fromAddress, IEnumerable<string> toAddresses,
            BlockchainPermissions permissions, decimal nativeAmount = 0M,
            string comment = null, string commentTo = null, int startBlock = 0, int endBlock = 0)
        {
            var stringifiedAddresses = StringifyValues(toAddresses);
            var permissionsAsString = FormatPermissions(permissions);
            return ExecuteAsync<string>("grantfrom", 0, fromAddress, stringifiedAddresses, permissionsAsString);
        }

        /// <summary>
        /// Returns a list of all permissions which have been explicitly granted to addresses.
        /// </summary>
        /// <param name="permissions">To list information about specific global permissions, set permissions to one of connect, send, receive, issue, mine, activate, admin, or a comma-separated list thereof. Omit or pass * or all to list all global permissions</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<ListPermissionsResponse>>> ListPermissions(BlockchainPermissions permissions)
        {
            var permissionsAsString = FormatPermissions(permissions);
            return ExecuteAsync<List<ListPermissionsResponse>>("listpermissions", 0, permissionsAsString);
        }

        /// <summary>
        /// Revokes permissions from addresses
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="permissions"></param>
        /// <param name="nativeAmount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <returns>Returns the txid of transaction revoking the permissions.</returns>
        public Task<JsonRpcResponse<string>> RevokeAsync(IEnumerable<string> addresses,
            BlockchainPermissions permissions, decimal nativeAmount = 0M, string comment = null,
            string commentTo = null)
        {
            var stringifiedAddresses = StringifyValues(addresses);
            var permissionsAsString = FormatPermissions(permissions);
            return ExecuteAsync<string>("revoke", 0, stringifiedAddresses, permissionsAsString);
        }

        /// <summary>
        /// This works like revoke, but with control over the from-address used to revoke the permissions.
        /// It is useful if the node has multiple addresses with administrator permissions.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddresses"></param>
        /// <param name="permissions"></param>
        /// <param name="nativeAmount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <returns>Returns the txid of transaction revoking the permissions.</returns>
        /// <see cref="RevokeAsync"/>
        public Task<JsonRpcResponse<string>> RevokeFromAsync(string fromAddress, IEnumerable<string> toAddresses,
            BlockchainPermissions permissions, decimal nativeAmount = 0M, string comment = null,
            string commentTo = null)
        {
            var stringifiedAddresses = StringifyValues(toAddresses);
            var permissionsAsString = FormatPermissions(permissions);
            return ExecuteAsync<string>("revokefrom", 0, fromAddress, stringifiedAddresses, permissionsAsString);
        }

        /// <summary>
        /// Format Permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns>Return a comma-separated list of permissions</returns>
        private static string FormatPermissions(BlockchainPermissions permissions)
        {
            StringBuilder builder = new StringBuilder();
            if ((int)(permissions & BlockchainPermissions.Connect) != 0)
                builder.Append("connect");
            if ((int)(permissions & BlockchainPermissions.Send) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("send");
            }
            if ((int)(permissions & BlockchainPermissions.Receive) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("receive");
            }
            if ((int)(permissions & BlockchainPermissions.Issue) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("issue");
            }
            if ((int)(permissions & BlockchainPermissions.Mine) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("mine");
            }
            if ((int)(permissions & BlockchainPermissions.Admin) != 0)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append("admin");
            }

            return builder.ToString();
        }

        #endregion

        #region -= Asset management =-

        /// <summary>
        /// Creates a new asset on the blockchain, sending the initial qty units to address.
        /// </summary>
        /// <param name="issueAddress"></param>
        /// <param name="assetName"></param>
        /// <param name="quantity"></param>
        /// <param name="units"></param>
        /// <returns>Returns the txid of the issuance transaction.</returns>
        /// <remarks>
        /// To create an asset with the default behavior, use an asset name only for name|params. For more control, 
        /// use an object such as {"name":"asset1","open":true}. 
        /// If open is true then additional units can be issued in future by the same key which signed the original issuance, 
        /// via the issuemore or issuemorefrom command. The smallest transactable unit is given by units, e.g. 0.01. 
        /// If the chain uses a native currency, you can send some with the new asset using the native-amount parameter.
        /// </remarks>
        public Task<JsonRpcResponse<string>> IssueAsync(string issueAddress, string assetName, int quantity,
            decimal units)
        {
            return ExecuteAsync<string>("issue", 0, issueAddress, assetName, quantity, units);
        }

        /// <summary>
        /// This works like issue, but with control over the from-address used to issue the asset. It is useful if the node has multiple addresses with issue permissions.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetName"></param>
        /// <param name="quantity"></param>
        /// <param name="units"></param>
        /// <returns>Returns the txid of the issuance transaction.</returns>
        /// <see cref="IssueAsync"/>
        public Task<JsonRpcResponse<string>> IssueFromAsync(string fromAddress, string toAddress, string assetName,
            int quantity, decimal units)
        {
            return ExecuteAsync<string>("issuefrom", 0, fromAddress, toAddress, assetName, quantity, units);
        }

        /// <summary>
        /// Issues quantity additional units of asset, sending them to address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="asset">The assetName can be specified using its name</param>
        /// <param name="quantity"></param>
        /// <returns>Returns the txid of the issuance transaction.</returns>
        public Task<JsonRpcResponse<string>> IssueMoreAsync(string address, string asset, int quantity)
        {
            return ExecuteAsync<string>("issuemore", 0, address, asset, quantity);
        }

        /// <summary>
        /// This works like issuemore, but with control over the from-address used.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetName"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <see cref="IssueMoreAsync"/>
        public Task<JsonRpcResponse<string>> IssueMoreFromAsync(string fromAddress, string toAddress, string assetName,
            int quantity)
        {
            return ExecuteAsync<string>("issuemorefrom", 0, fromAddress, toAddress, assetName, quantity);
        }

        /// <summary>
        /// Returns information about assets issued on the blockchain. 
        /// Pass an asset name, ref or issuance txid in assets to retrieve information about one asset only, an array thereof for multiple assets, or * for all assets. 
        /// In assets with multiple issuance events, the top-level issuetxid and details fields refer to the first issuance only.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<AssetResponse>>> ListAssetsAsync()
        {
            return ExecuteAsync<List<AssetResponse>>("listassets", 0);
        }

        #endregion

        #region  -= Querying wallet balances and transactions =-

        /// <summary>
        /// Returns a list of all the asset balances for address in this node’s wallet, with at least minconf confirmations
        /// </summary>
        /// <param name="address"></param>
        /// <param name="minconf"></param>
        /// <param name="includeLocked">Use includeLocked to include unspent outputs which have been locked</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<AssetBalanceResponse>>> GetAddressBalancesAsync(string address,
            int minconf = 1, bool includeLocked = false)
        {
            return ExecuteAsync<List<AssetBalanceResponse>>("getaddressbalances", 0, address, minconf, includeLocked);
        }

        /// <summary>
        /// Provides information about transaction txid related to address in this node’s wallet, including how it affected that address’s balance.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="txId"></param>
        /// <param name="verbose">Use verbose to provide details of transaction inputs and outputs.</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<AddressTransactionResponse>> GetAddressTransactionAsync(string address, string txId,
            bool verbose = false)
        {
            return ExecuteAsync<AddressTransactionResponse>("getaddresstransaction", 0, address, txId, verbose);
        }

        /// <summary>
        /// Returns a list of all the asset balances in this node’s wallet, with at least minconf confirmations.
        /// </summary>
        /// <param name="minconf"></param>
        /// <param name="watchOnly">Use watchOnly to include the balance of watch-only addresses</param>
        /// <param name="locked">Use locked to include unspent outputs which have been locked</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<AssetBalanceResponse>>> GetTotalBalancesAsync(int minconf = 1,
            bool watchOnly = false, bool locked = false)
        {
            return ExecuteAsync<List<AssetBalanceResponse>>("gettotalbalances", 0, minconf, watchOnly, locked);
        }

        /// <summary>
        /// Lists information about the count most recent transactions related to address in this node’s wallet, including how they affected that address’s balance
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <param name="skip">Use skip to go back further in history</param>
        /// <param name="verbose">Use verbose to provide details of transaction inputs and outputs</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<AddressTransactionResponse>>> ListAddressTransactionsAsync(string address,
            int count = 10, int skip = 0, bool verbose = false)
        {
            return ExecuteAsync<List<AddressTransactionResponse>>("listaddresstransactions", 0, address, count, skip,
                verbose);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <param name="watchOnly"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<TransactionResponse>>> ListTransactionsAsync(string account = null, int count = 10, int skip = 0, bool watchOnly = false)
        {
            return ExecuteAsync<List<TransactionResponse>>("listtransactions", 0, account ?? string.Empty, count, skip, watchOnly);
        }

        #endregion

        #region -= Sending one-way payments =-

        /// <summary>
        /// Sends one or more assets to address, returning the txid. In Bitcoin Core, the amount field is the quantity of the bitcoin currency. 
        /// For MultiChain, an {"asset":qty, ...} object can be used for amount, in which each asset is an asset name, 
        /// ref or issuance txid, and each qty is the quantity of that asset to send.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="assetName"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <returns>Return the txid</returns>
        public Task<JsonRpcResponse<string>> SendToAddressAsync(string address, string assetName, decimal amount,
            string comment = null, string commentTo = null)
        {
            var theAmount = new Dictionary<string, object> { [assetName] = amount };
            return ExecuteAsync<string>("sendtoaddress", 0, address, theAmount, comment ?? string.Empty,
                commentTo ?? string.Empty);
        }

        /// <summary>
        /// Sends qty of asset to address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="assetName">The asset can be specified using its name</param>
        /// <param name="quantity"></param>
        /// <param name="nativeAmount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <returns>Return the txid</returns>
        public Task<JsonRpcResponse<string>> SendAssetToAddressAsync(string address, string assetName, decimal quantity,
            int nativeAmount = 0, string comment = null, string commentTo = null)
        {
            return ExecuteAsync<string>("sendassettoaddress", 0, address, assetName, quantity, nativeAmount,
                comment ?? string.Empty, commentTo ?? string.Empty);
        }

        /// <summary>
        /// This works like sendasset, but with control over the from-address whose funds are used. 
        /// Any change from the transaction is sent back to from-address.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetName"></param>
        /// <param name="quantity"></param>
        /// <param name="nativeAmount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> SendAssetFromAsync(string fromAddress, string toAddress, string assetName,
            decimal quantity, int nativeAmount = 0, string comment = null, string commentTo = null)
        {
            return ExecuteAsync<string>("sendassetfrom", 0, fromAddress, toAddress, assetName, quantity, nativeAmount,
                comment ?? string.Empty, commentTo ?? string.Empty);
        }

        /// <summary>
        /// This works like send, but with control over the from-address whose funds are used. Any change from the transaction is sent back to from-address.
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="confirmations"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> SendFromAsync(string fromAccount, string toAddress, decimal amount,
            int confirmations = 1, string comment = null, string commentTo = null)
        {
            return ExecuteAsync<string>("sendfrom", 0, fromAccount ?? string.Empty, toAddress, amount,
                confirmations, comment ?? string.Empty, commentTo ?? string.Empty);
        }

        /// <summary>
        /// This works like send, but with an additional data-only transaction output. 
        /// To include raw data, pass a data-hex hexadecimal string. 
        /// To publish the data to a stream, pass an object {"for":stream,"key":"...","data":"..."} where stream is a stream name, 
        /// ref or creation txid, the key is in text form, and the data is hexadecimal.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="assetName"></param>
        /// <param name="amount"></param>
        /// <param name="dataHex"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> SendWithMetadataAsync(string address, string assetName, decimal amount,
            byte[] dataHex)
        {
            var theAmount = new Dictionary<string, object> { [assetName] = amount };
            return ExecuteAsync<string>("sendwithmetadata", 0, address, theAmount, FormatHex(dataHex));
        }

        /// <summary>
        /// This works like sendwithdata, but with control over the from-address whose funds are used. 
        /// Any change from the transaction is sent back to from-address.
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetName"></param>
        /// <param name="amount"></param>
        /// <param name="dataHex"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> SendWithMetadataFromAsync(string fromAddress, string toAddress,
            string assetName, decimal amount, byte[] dataHex)
        {
            var theAmount = new Dictionary<string, object> { [assetName] = amount };
            return ExecuteAsync<string>("sendwithmetadatafrom", 0, fromAddress, toAddress, theAmount,
                FormatHex(dataHex));
        }

        #endregion

        #region -= Atomic exchange transactions =-

        // appendrawexchange

        // completerawexchange

        // createrawexchange

        // decoderawexchange

        // disablerawtransaction

        // preparelockunspent

        // preparelockunspentfrom

        #endregion

        #region -= Stream management =-

        /// <summary>
        /// Creates a new stream on the blockchain called name.
        /// </summary>
        /// <param name="type">For now, always pass the value "stream" in the type parameter – this is designed for future functionality. </param>
        /// <param name="name">Name of stream</param>
        /// <param name="open">If open is true then anyone with global send permissions can publish to the stream, otherwise publishers must be explicitly granted per-stream write permissions.</param>
        /// <returns></returns>
        public Task<JsonRpcResponse<object>> Create(string type, string name, bool open)
        {
            return ExecuteAsync<object>("create", 1, type, name, open);
        }

        /// <summary>
        /// Returns information about streams created on the blockchain. 
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<ListStreamsResponse>>> ListStreams()
        {
            return ExecuteAsync<List<ListStreamsResponse>>("liststreams", 0);
        }

        #endregion

        #region -= Publishing stream items =-

        /// <summary>
        /// Publishes an item in stream.
        /// </summary>
        /// <param name="streamName">stream name</param>
        /// <param name="key">provided in text form</param>
        /// <param name="message">data-hex in hexadecimal</param>
        /// <returns></returns>
        public async Task<JsonRpcResponse<object>> Publish(string streamName, string key, string message)
        {
            var bs = Encoding.UTF8.GetBytes(message);

            return await ExecuteAsync<object>("publish", 1, streamName, key, FormatHex(bs));
        }

        #endregion

        #region -= Managing stream and asset subscriptions =-

        /// <summary>
        /// Instructs the node to start tracking one or more asset(s) or stream(s). 
        /// </summary>
        /// <param name="names"></param>
        /// <returns>Returns null if successful.</returns>
        public Task<JsonRpcResponse<object>> Subscribe(string names)
        {
            return ExecuteAsync<object>("subscribe", 1, names);
        }

        // unsubscribe

        #endregion

        #region -= Querying subscribed assets =-

        // getassettransaction

        // listassettransactions

        #endregion

        #region -= Querying subscribed streams =-

        /// <summary>
        /// Retrieves a specific item with txid from stream, passed as a stream name, ref or creation txid, to which the node must be subscribed
        /// </summary>
        /// <param name="streamName">Stream name</param>
        /// <param name="txid">creation txid</param>
        /// <param name="verbose">Set verbose to true for additional information about the item’s transaction</param>
        /// <returns>If an item’s data is larger than the maxshowndata runtime parameter, it will be returned as an object whose fields can be used with gettxoutdata.</returns>
        /// <see cref="GetTxOutDataAsync"/>
        public Task<JsonRpcResponse<ListStreamItemsResponse>> GetStreamItem(string streamName, string txid,
            bool verbose = false)
        {
            return ExecuteAsync<ListStreamItemsResponse>("getstreamitem", 1, streamName, txid, verbose);
        }

        /// <summary>
        /// This is particularly useful if a stream item’s data is larger than the maxshowndata runtime parameter.
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="vout"></param>
        /// <returns>Returns the data embedded in output vout of transaction txid, in hexadecimal.</returns>
        public Task<JsonRpcResponse<string>> GetTxOutDataAsync(string txId, int vout = 0)
        {
            return ExecuteAsync<string>("gettxoutdata", 0, txId, vout);
        }

        /// <summary>
        /// This works like liststreamitems, but listing items with the given key only.
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="key"></param>
        /// <param name="verbose"></param>
        /// <param name="count"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        /// <see cref="ListStreamItems"/>
        public Task<JsonRpcResponse<List<ListStreamItemsResponse>>> ListStreamKeyItems(string streamName, string key,
            bool verbose = false, int count = 1, int start = -1)
        {
            return ExecuteAsync<List<ListStreamItemsResponse>>("liststreamkeyitems", 1, streamName, key, verbose, count,
                start);
        }

        /// <summary>
        /// Provides information about keys in stream, passed as a stream name, ref or creation txid.
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="keys">Pass a single key in keys to retrieve information about one key only, pass an array for multiple keys, or * for all keys.</param>
        /// <param name="verbose">Set verbose to true to include information about the first and last item with each key shown.</param>
        /// <param name="count"></param>
        /// <param name="start"></param>
        /// <param name="localOdering"></param>
        /// <returns></returns>
        /// <see cref="ListStreamItems"/>
        public Task<JsonRpcResponse<List<ListStreamKeysResponse>>> ListStreamKeys(string streamName, string keys = "*",
            bool verbose = false, int count = 10000, int start = 0, bool localOdering = false)
        {
            return ExecuteAsync<List<ListStreamKeysResponse>>("liststreamkeys", 1, streamName, keys, verbose, count,
                start, localOdering);
        }

        /// <summary>
        /// Lists items in stream, passed as a stream name, ref or creation txid.
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="start">Use count and start to retrieve part of the list only, with negative start values (like the default) indicating the most recent items.</param>
        /// <param name="count">Use count and start to retrieve part of the list only, with negative start values (like the default) indicating the most recent items.</param>
        /// <param name="verbose">Set verbose to true for additional information about each item’s transaction.</param>
        /// <param name="localOdering">Set local-ordering to true to order items by when first seen by this node, rather than their order in the chain. </param>
        /// <returns></returns>
        /// <remarks>If an item’s data is larger than the maxshowndata runtime parameter, it will be returned as an object whose fields can be used with gettxoutdata.</remarks>
        public Task<JsonRpcResponse<List<ListStreamItemsResponse>>> ListStreamItems(string streamName, int start = 0,
            int count = 10000, bool verbose = false, bool localOdering = false)
        {
            return ExecuteAsync<List<ListStreamItemsResponse>>("liststreamitems", 1, streamName, verbose, count, start,
                localOdering);
        }

        /// <summary>
        /// This works like liststreamitems, but listing items published by the given address only.
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<ListStreamItemsResponse>>> ListStreamPublisherItems(string streamName,
            string address)
        {
            return ExecuteAsync<List<ListStreamItemsResponse>>("liststreampublisheritems", 1, streamName, address);
        }

        // liststreampublishers

        #endregion

        #region -= Managing wallet unspent outputs =-

        // combineunspent

        /// <summary>
        /// Returns a list of locked unspent transaction outputs in the wallet. 
        /// These will not be used when automatically selecting the outputs to spend in a new transaction.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<string>>> ListLockUnspentAsync()
        {
            return ExecuteAsync<List<string>>("listlockunspent", 0);
        }

        /// <summary>
        /// Returns a list of unspent transaction outputs in the wallet, with between minconf and maxconf confirmations. 
        /// </summary>
        /// <param name="minConf"></param>
        /// <param name="maxConf"></param>
        /// <param name="addresses">If the third parameter is provided, only outputs which pay an address in this array will be included.</param>
        /// <returns></returns>
        /// <remarks>For a MultiChain blockchain, each transaction output includes assets and permissions fields listing any assets or permission changes encoded within that output. </remarks>
        public Task<JsonRpcResponse<List<UnspentResponse>>> ListUnspentAsync(int minConf = 1, int maxConf = 999999,
            IEnumerable<string> addresses = null)
        {
            return ExecuteAsync<List<UnspentResponse>>("listunspent", 0, minConf, maxConf);
        }

        // lockunspent

        #endregion

        #region -= Working with raw transactions =-

        // appendrawchange

        // appendrawdata

        // createrawtransaction

        /// <summary>
        /// This works like createrawtransaction, except it automatically selects the transaction inputs from those belonging to from-address, to cover the appropriate amounts. 
        /// One or more change outputs going back to from-address will also be added to the end of the transaction.
        /// </summary>
        /// <param name="addressFrom"></param>
        /// <param name="addressTo"></param>
        /// <param name="assetName">Use "" as the asset inside this object to specify a quantity of the native blockchain currency.</param>
        /// <param name="amount"></param>
        /// <param name="data">The optional data array adds one or more metadata outputs to the transaction, where each element is a raw hexadecimal string or object, formatted as passed to appendrawdata.</param>
        /// <returns>The raw transaction hexadecimal is returned</returns>
        /// <remarks>
        /// In Bitcoin Core, each amount field is a quantity of the bitcoin currency. 
        /// For MultiChain, an {"asset":qty, ...} object can be used for amount, in which each asset is an asset name, ref or issuance txid, 
        /// and each qty is the quantity of that asset to send (see native assets).
        /// </remarks>
        public Task<JsonRpcResponse<string>> CreateRawSendFromAync(string addressFrom, string addressTo, string assetName, decimal amount, string data)
        {
            string[] meta = null;
            if (!string.IsNullOrEmpty(data))
            {
                meta = new string[1];
                var bs = Encoding.UTF8.GetBytes(data);
                meta[0] = FormatHex(bs);
            }

            var theAmount = new Dictionary<string, decimal> { [assetName] = amount };
            var thePayment = new Dictionary<string, object> { [addressTo] = theAmount };
            return ExecuteAsync<string>("createrawsendfrom", 0, addressFrom, thePayment, meta);
        }

        /// <summary>
        /// Returns a JSON object describing the serialized transaction in tx-hex.
        /// </summary>
        /// <param name="txHex"></param>
        /// <returns></returns>
        /// <remarks>
        /// For a MultiChain blockchain, each transaction output includes assets and permissions fields listing any assets or permission changes encoded within that output. 
        /// There will also be a data field listing the content of any OP_RETURN outputs in the transaction.
        /// </remarks>
        public Task<JsonRpcResponse<VerboseTransactionResponse>> DecodeRawTransactionAsync(string txHex)
        {
            return ExecuteAsync<VerboseTransactionResponse>("decoderawtransaction", 0, txHex);
        }

        /// <summary>
        /// Validates the raw transaction in tx-hex and transmits it to the network, returning the txid. 
        /// </summary>
        /// <param name="txHex"></param>
        /// <returns></returns>
        /// <remarks>
        /// The raw transaction can be created using createrawtransaction, (optionally) appendrawdata and signrawtransaction, or else createrawexchange and appendrawexchange.
        /// </remarks>
        public Task<JsonRpcResponse<string>> SendRawTransactionAsync(string txHex)
        {
            return ExecuteAsync<string>("sendrawtransaction", 0, txHex);
        }

        /// <summary>
        /// Signs the raw transaction in tx-hex, often provided by a previous call to createrawtransaction or createrawsendfrom.
        /// </summary>
        /// <param name="txHex"></param>
        /// <returns>Returns a raw hexadecimal transaction in the hex field alongside a complete field stating whether it is now completely signed.</returns>
        /// <remarks>
        /// If complete, the transaction can be broadcast to the network using sendrawtransaction. 
        /// If not, it can be passed to other parties for additional signing. 
        /// To create chains of unbroadcast transactions, pass an optional array of {parent-output} objects, each of which takes the form {"txid":txid,"vout":n,"scriptPubKey":hex}. 
        /// To sign using (only) private keys which are not in the node’s wallet, pass an array of "private-key" strings, formatted as per the output of dumpprivkey. 
        /// To sign only part of the transaction, use the sighashtype parameter to control the signature hash type.
        /// </remarks>
        public Task<JsonRpcResponse<string>> SignRawTransactionAsync(string txHex)
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }

        #endregion

        #region -= Peer-to-peer connections =-

        /// <summary>
        /// Manually adds or removes a peer-to-peer connection
        /// </summary>
        /// <param name="address">The ip can be a hostname, IPv4 address, IPv4-as-IPv6 address or IPv6 address.</param>
        /// <param name="command">The command parameter should be one of add (to manually queue a node for the next available slot), remove (to remove a node), or onetry (to immediately connect to a node even if a slot is not available).</param>
        /// <returns></returns>
        /// <remarks>For the entire ip:port you can also use the part after the @ symbol of the other node’s nodeaddress, as given by the getinfo command.</remarks>
        public Task<JsonRpcResponse<string>> AddNodeAsync(string address, AddNodeCommand command)
        {
            return ExecuteAsync<string>("addnode", 0, address, command.ToString().ToLower());
        }

        /// <summary>
        /// returns a list of added nodes only.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<string>>> GetAddedNodeInfoAsync()
        {
            return ExecuteAsync<List<string>>("getaddednodeinfo", 0, false);
        }

        /// <summary>
        /// Returns information about a node which was added using addnode
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<string>>> GetAddedNodeInfoAsync(string node)
        {
            return ExecuteAsync<List<string>>("getaddednodeinfo", 0, false, node);
        }

        /// <summary>
        /// Returns information about the network ports to which the node is connected, and its local IP addresses.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<NetworkInfoResponse>> GetNetworkInfoAsync()
        {
            return ExecuteAsync<NetworkInfoResponse>("getnetworkinfo", 0);
        }

        /// <summary>
        /// Returns information about the other nodes to which this node is connected. 
        /// If this is a MultiChain blockchain, includes handshake and handshakelocal fields showing the remote and local address used during the handshaking for that connection.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<PeerResponse>>> GetPeerInfoAsync()
        {
            return ExecuteAsync<List<PeerResponse>>("getpeerinfo", 0);
        }

        /// <summary>
        /// Sends a ping message to all connected nodes to measure network latency and backlog. The results are received asynchronously and retrieved from the pingtime field of the response to getpeerinfo.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> PingAsync()
        {
            return ExecuteAsync<string>("ping", 0);
        }

        #endregion

        #region -= Messaging signing and verification =-

        // signmessage

        // verifymessage

        #endregion

        #region -= Querying the blockchain =-

        /// <summary>
        /// Returns information about the block with hash (retrievable from getblockhash) or at the given height in the active chain.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public Task<JsonRpcResponse<string>> GetBlockAsync(string hash, bool verbose = false)
        {
            return ExecuteAsync<string>("getblock", 0, hash, verbose);
        }

        /// <summary>
        /// Returns information about the blockchain, including the bestblockhash of the most recent block on the active chain, 
        /// which can be compared across nodes to check if they are perfectly synchronized.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<BlockchainInfoResponse>> GetBlockchainInfoAsync()
        {
            return ExecuteAsync<BlockchainInfoResponse>("getblockchaininfo", 0);
        }

        /// <summary>
        /// eturns the hash of the block at the given height.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <remarks>This can be passed to getblock to get information about the block.</remarks>
        public Task<JsonRpcResponse<string>> GetBlockHashAsync(int height)
        {
            return ExecuteAsync<string>("getblockhash", 0, height);
        }

        /// <summary>
        /// Returns information about the memory pool, which contains transactions that the node has seen and validated,
        /// but which have not yet been confirmed on the active chain. 
        /// If the memory pool is growing continuously, this suggests that 
        /// transactions are being generated faster than the network is able to process them.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<MempoolInfoResponse>> GetMempoolInfoAsync()
        {
            return ExecuteAsync<MempoolInfoResponse>("getmempoolinfo", 0);
        }

        /// <summary>
        /// Returns a list of transaction IDs which are in the node’s memory pool (see getmempoolinfo).
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<List<object>>> GetRawMempoolAsync()
        {
            return ExecuteAsync<List<object>>("getrawmempool", 0);
        }

        /// <summary>
        /// If verbose is 1, returns a JSON object describing transaction txid.
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        /// <remarks>
        /// For a MultiChain blockchain, each transaction output includes assets and permissions fields listing any assets or permission changes encoded within that output. 
        /// There will also be a data field listing the content of any OP_RETURN outputs in the transaction.
        /// </remarks>
        public Task<JsonRpcResponse<VerboseTransactionResponse>> GetRawTransactionVerboseAsync(string txId, int verbose)
        {
            return ExecuteAsync<VerboseTransactionResponse>("getrawtransaction", 0, txId, verbose);
        }

        /// <summary>
        /// Returns details about an unspent transaction output vout of txid. 
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="vout"></param>
        /// <param name="unconfirmed">Set unconfirmed to true to include unconfirmed transaction outputs.</param>
        /// <returns></returns>
        /// <remarks>For a MultiChain blockchain, includes assets and permissions fields listing any assets or permission changes encoded within the output.</remarks>
        public Task<JsonRpcResponse<TxOutResponse>> GetTxOutAsync(string txId, int vout = 0, bool unconfirmed = false)
        {
            return ExecuteAsync<TxOutResponse>("gettxout", 0, txId, vout, unconfirmed);
        }

        #endregion

        #region -= Advanced wallet control =-

        /// <summary>
        /// Creates a backup of the wallet.dat file in which the node’s private keys and watch-only addresses are stored.
        /// </summary>
        /// <param name="filename">The backup is created in file filename.</param>
        /// <returns></returns>
        /// <remarks>Use with caution – any node with access to this file can perform any action restricted to this node’s addresses.</remarks>
        public Task<JsonRpcResponse<string>> BackupWalletAsync(string filename)
        {
            return ExecuteAsync<string>("backupwallet", 0, filename);
        }

        /// <summary>
        /// Returns the private key associated with address in this node’s wallet. 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <remarks>Use with caution – any node with access to this private key can perform any action restricted to the address.</remarks>
        public Task<JsonRpcResponse<string>> DumpPrivKeyAsync(string address)
        {
            return ExecuteAsync<string>("dumpprivkey", 0, address);
        }

        /// <summary>
        /// Dumps the entire set of private keys in the wallet into a human-readable text format in file filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <remarks>Use with caution – any node with access to this file can perform any action restricted to this node’s addresses.</remarks>
        public Task<JsonRpcResponse<string>> DumpWalletAsync(string filename)
        {
            return ExecuteAsync<string>("dumpwallet", 0, filename);
        }

        /// <summary>
        /// This encrypts the node’s wallet for the first time
        /// </summary>
        /// <param name="passphrase">using passphrase as the password for unlocking</param>
        /// <returns></returns>
        /// <remarks>
        /// Once encryption is complete, the wallet’s private keys can no longer be retrieved directly from the wallet.dat file on disk, 
        /// and MultiChain will stop and need to be restarted. Use with caution – once a wallet has been encrypted it cannot be permanently unencrypted, 
        /// and must be unlocked for signing transactions with the walletpassphrase command. In a permissioned blockchain, 
        /// MultiChain will also require the wallet to be unlocked before it can connect to other nodes, or sign blocks that it has mined.
        /// </remarks>
        public Task<JsonRpcResponse<string>> EncryptWalletAsync(string passphrase)
        {
            return ExecuteAsync<string>("encryptwallet", 0, passphrase);
        }

        /// <summary>
        /// Returns information about the node’s wallet, including the number of transactions (txcount) 
        /// and unspent transaction outputs (utxocount), the pool of pregenerated keys. 
        /// If the wallet has been encrypted and unlocked, it also shows when it is unlocked_until.
        /// </summary>
        /// <returns></returns>
        public Task<JsonRpcResponse<WalletInfoResponse>> GetWalletInfoAsync()
        {
            return ExecuteAsync<WalletInfoResponse>("getwalletinfo", 0);
        }

        /// <summary>
        /// Adds a privkey private key (or an array thereof) to the wallet, together with its associated public address.
        /// </summary>
        /// <param name="key">private key (or an array thereof)</param>
        /// <param name="account"></param>
        /// <param name="rescan">If rescan is true, the entire blockchain is checked for transactions relating to all addresses in the wallet, including the added ones.</param>
        /// <returns>Returns null if successful.</returns>
        public Task<JsonRpcResponse<string>> ImportPrivKey(string key, string account = null, bool rescan = true)
        {
            return ExecuteAsync<string>("importprivkey", 0, key, account ?? string.Empty, rescan);
        }

        /// <summary>
        /// Imports the entire set of private keys which were previously dumped (using dumpwallet) into file filename. 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <remarks>The entire blockchain will be rescanned for transactions relating to the addresses corresponding with these private keys.</remarks>
        public Task<JsonRpcResponse<string>> ImportWallet(string filename)
        {
            return ExecuteAsync<string>("importwallet", 0, filename);
        }

        // walletlock

        // walletpassphrase

        // walletpassphrasechange

        #endregion

        #region -= Advanced node control =-

        // clearmempool

        // pause

        // resume

        // setlastblock

        #endregion

        #region -= Utilities =-

        /// <summary>
        /// Convert hex string to bytes array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] ParseHexString(string hex)
        {
            var bs = new List<byte>();
            for (var index = 0; index < hex.Length; index += 2)
                bs.Add(byte.Parse(hex.Substring(index, 2), NumberStyles.HexNumber));
            return bs.ToArray();
        }

        /// <summary>
        /// Convert bytes array to hex string
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        private static string FormatHex(byte[] bs)
        {
            var builder = new StringBuilder();
            foreach (var b in bs)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        /// <summary>
        /// String join 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private string StringifyValues(IEnumerable<string> values)
        {
            return string.Join(",", values);
        }

        #endregion

        /// <summary>
        /// Execute RPC-call async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task<JsonRpcResponse<T>> ExecuteAsync<T>(string method, int id, params object[] args)
        {
            var ps = new JsonRpcRequest()
            {
                Method = method,
                Params = args,
                ChainName = ChainName,
                Id = id
            };

            // defer...
            //OnExecuting(new EventArgs<JsonRpcRequest>(ps));

            var jsonOut = JsonConvert.SerializeObject(ps.Values);
            var url = ServiceUrl;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                //Timeout after 30000 ms
                request.Timeout = 30000;
                request.Credentials = GetCredentials();
                request.Method = "POST";

                var bs = Encoding.UTF8.GetBytes(jsonOut);
                using (var stream = request.GetRequestStream())
                    stream.Write(bs, 0, bs.Length);

                // get the response...
                var response = (HttpWebResponse)await request.GetResponseAsync();
                string jsonIn = null;
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            jsonIn = reader.ReadToEnd();
                        }
                    }
                }
                response.Close();
                JsonRpcResponse<T> theResult;
                try
                {
                    theResult = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(jsonIn);
                }
                catch
                {
                    const string errorData = "{\"result\":null,\"error\":{\"code\":10001,\"message\":\"\"}}";
                    throw new MultiChainInvalidOperationException("Deserialize JSON object error.", errorData, url, jsonIn);
                }
                theResult.RawJson = jsonIn;
                return theResult;
            }
            catch (Exception ex)
            {
                var walk = ex;
                string errorData = null;
                while (walk != null)
                {
                    if (walk is MultiChainInvalidOperationException)
                        throw walk;

                    if (walk is WebException)
                    {
                        var webEx = (WebException)walk;
                        if (webEx.Status == WebExceptionStatus.Timeout)
                        {
                            errorData = $"Method {method} have been processing too long.";
                        }
                        else if (webEx.Response != null)
                        {
                            using (var stream = webEx.Response.GetResponseStream())
                                if (stream != null)
                                    errorData = new StreamReader(stream).ReadToEnd();
                        }

                        break;
                    }

                    walk = walk.InnerException;
                }

                throw new MultiChainInvalidOperationException("JSON-RPC call error", errorData, url, jsonOut);
            }
        }

        //protected virtual void OnExecuting(EventArgs<JsonRpcRequest> e)
        //{
        //    Executing?.Invoke(this, e);
        //}

        private string ServiceUrl
        {
            get
            {
                var protocol = "https";
                if (!UseSsl)
                    protocol = "http";
                return $"{protocol}://{Hostname}:{Port}/";
            }
        }

        private ICredentials GetCredentials()
        {
            return HasCredentials ? new NetworkCredential(Username, Password) : null;
        }

        public bool HasCredentials => !(string.IsNullOrEmpty(Username));
    }
}