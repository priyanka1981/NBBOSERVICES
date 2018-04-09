using System;
namespace NbboService
{
    public class Order
    {
        
        public Order()
        {
        }

        private string name; //pk
        private double ask;
        private long askSize;
        private double bid;
        private long bidSize;
        private double last;
        private string exchange;//pk

        public string Exchange { get => exchange; set => exchange = value; }
        public double Last { get => last; set => last = value; }
        public long BidSize { get => bidSize; set => bidSize = value; }
        public double Bid { get => bid; set => bid = value; }
        public long AskSize { get => askSize; set => askSize = value; }
        public double Ask { get => ask; set => ask = value; }
        public string Name { get => name; set => name = value; }
    }
}
