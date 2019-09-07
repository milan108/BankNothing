using System;
using System.Collections.Generic;
using System.Text;

namespace currency.blockchain
{
    class Block
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PrevHash { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int Proof { get; set; }
        public override string ToString()
        {
            return $"{Index} [{Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}] Proof: {Proof} | PrevHash: {PrevHash} | Trx: {Transactions.Count}";
        }
    }
}
