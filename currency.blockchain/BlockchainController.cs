using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace currency.blockchain
{

    [ApiController]
    [Route("")]
    public class BlockchainController : ControllerBase
    {
        private static Blockchain blockchain = new Blockchain();

        [HttpGet("/mine")]
        public string Mine()
        {
            return blockchain.Mine();
        }

        [HttpPost("/transaction/{sender}/{recipient}/{amount}")]
        public string NewTransaction(string sender, string recipient, int amount)
        {
            int index = blockchain.CreateTransaction(sender, recipient, amount);
            return $"Transaction added to block {index}";
        }

        [HttpGet("/chain")]
        public string Chain()
        {
            return blockchain.GetFullChain();
        }
    }
}
