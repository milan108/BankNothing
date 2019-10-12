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
    public class Blockchain
    {
        private List<Block> chain = new List<Block>();
        private List<Transaction> currentTransactions = new List<Transaction>();
        private Block lastBlock => chain.Last();
        private List<Node> nodes = new List<Node>();

        public Blockchain()
        {
            CreateNewBlock(proof: 100, prevHash: "1"); //genesis
        }

        public Block CreateNewBlock(int proof, string prevHash = null)
        {
            Block block = new Block
            {
                Index = chain.Count,
                Timestamp = DateTime.UtcNow,
                PrevHash = prevHash ?? GetHash(lastBlock),
                Transactions = currentTransactions,
                Proof = proof
            };
            currentTransactions.Clear();
            chain.Add(block);

            return block;
        }

        private string GetHash(Block block)
        {
            string blockText = JsonConvert.SerializeObject(block);
            return GetSha256(blockText);
        }

        private string GetSha256(string data)
        {
            var sha256 = new SHA256Managed();
            var hashBuilder = new StringBuilder();

            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] hash = sha256.ComputeHash(bytes);

            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");

            return hashBuilder.ToString();
        }

        private int CreateProofOfWork(int lastProof, string previousHash)
        {
            int proof = 0;
            while (!IsValidProof(lastProof, proof, previousHash))
                proof++;

            return proof;
        }

        private bool IsValidProof(int lastProof, int proof, string previousHash)
        {
            string guess = $"{lastProof}{proof}{previousHash}";
            string result = GetSha256(guess);
            return result.StartsWith("0000");
        }

        private bool IsValidChain(List<Block> chain)
        {
            int currentIndex = 1;
            var lastBlock = chain.First();
            while (currentIndex < chain.Count)
            {
                var currentBlock = chain.ElementAt(currentIndex);

                if (currentBlock.PrevHash != lastBlock.PrevHash)
                    return false;

                if (!IsValidProof(lastBlock.Proof, currentBlock.Proof, lastBlock.PrevHash))
                    return false;

                lastBlock = currentBlock;
                currentIndex++;
            }
            return true;
        }

        public int CreateTransaction(string sender, string recipient, int amount)
        {
            Transaction newTransaction = new Transaction { Sender = sender, Recipient = recipient, Amount = amount };
            currentTransactions.Add(newTransaction);
            return lastBlock != null ? lastBlock.Index + 1 : 0;
        }

        private void RegisterNode(string address)
        {
            nodes.Add(new Node { Address = new Uri(address) });
        }

        internal string RegisterNodes(string[] nodes)
        {
            var builder = new StringBuilder();
            foreach (string node in nodes)
            {
                string url = $"http://{node}";
                RegisterNode(url);
                builder.Append($"{url}, ");
            }

            builder.Insert(0, $"{nodes.Count()} new nodes have been added: ");
            string result = builder.ToString();
            return result.Substring(0, result.Length - 2);
        }

        private bool ResolveConflicts()
        {
            List<Block> newChain = null;
            int maxLength = chain.Count;

            foreach (Node node in nodes)
            {
                var url = new Uri(node.Address, "/chain");
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var model = new
                    {
                        chain = new List<Block>(),
                        length = 0
                    };
                    string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var data = JsonConvert.DeserializeAnonymousType(json, model);

                    if (data.chain.Count > chain.Count && IsValidChain(data.chain))
                    {
                        maxLength = data.chain.Count;
                        newChain = data.chain;
                    }
                }
            }

            if (newChain != null)
            {
                chain = newChain;
                return true;
            }

            return false;
        }

        internal string Consensus()
        {
            bool replaced = ResolveConflicts();
            string message = replaced ? "was replaced" : "is authoritive";

            var response = new
            {
                Message = $"Our chain {message}",
                Chain = chain
            };

            return JsonConvert.SerializeObject(response);
        }

        internal string GetFullChain()
        {
            var response = new
            {
                chain = chain.ToArray(),
                length = chain.Count
            };

            return JsonConvert.SerializeObject(response);
        }

        internal string Mine()
        {
            int proof = CreateProofOfWork(lastBlock.Proof, lastBlock.PrevHash);
            var block = CreateNewBlock(proof, lastBlock.PrevHash);
            var response = new
            {
                Message = "New Block Forged",
                Index = block.Index,
                Transactions = block.Transactions.ToArray(),
                Proof = block.Proof,
                PreviousHash = block.PrevHash
            };

            return JsonConvert.SerializeObject(response);
        }

    }
}