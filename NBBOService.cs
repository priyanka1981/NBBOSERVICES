using System;
//using Microsoft.Extensions.Caching;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using System.Linq;
using System.Threading;

namespace NbboService
{
    public class NBBOService : IService
    {
        readonly object locker = new object();

        MemoryCache cache = new MemoryCache("OrderCache");
        CacheItemPolicy policy = new CacheItemPolicy();

        Queue<Order> queue = new Queue<Order>();

        private static NBBOService _INSTANCE = new NBBOService();

        private NBBOService()
        {
            //
        }

        public static NBBOService getInstance() {
            return _INSTANCE;
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        // rest end point
        public object GetMarketData(string name)
        {
            // get the data from cache1 for ticker - name IBM
            HashSet<string> exchanges = (HashSet<string>)cache.Get(name);
            List<Order> orders = new List<Order>();
            foreach (string exchange in exchanges)
            {
                Order o = (Order)cache.Get(name + exchange);
                orders.Add(o);
            }

            return orders;
        }

        // rest end point
        public object GetNbboData(string name)
        {
            // get the data for security - name from cache2
            return cache.Get("NBBO" + name);
        }

        public void OnTick(string data)
        {
            /// "Name=IBM,Ask=5.34,AskSize=100,Bid=4.11,BidSize=200,Last=3.4,Exch=NYSE";
            // Console.WriteLine(Thread.CurrentThread.Name + "data {0}",data);
            string[] res = Regex.Split(data, ",");
            Order _order = new Order();
            Array.ForEach(res, x =>
            {
                string[] items = Regex.Split(x, "=");
                switch (items[0])
                {
                    case "Name":
                        _order.Name = items[1].ToString();
                        break;
                    case "Ask":
                        _order.Ask = Convert.ToDouble(items[1]);
                        _order.Ask = Convert.ToDouble(_order.Ask.ToString("#.#"));
                        break;
                    case "AskSize":
                        _order.AskSize = Convert.ToInt64(items[1]);
                        break;
                    case "Bid":
                        _order.Bid = Convert.ToDouble(items[1]);
                        _order.Bid = Convert.ToDouble(_order.Bid.ToString("#.#"));
                        break;
                    case "BidSize":
                        _order.BidSize = Convert.ToInt64(items[1]);
                        break;
                    case "Last":
                        _order.Last = Convert.ToDouble(items[1]);
                        break;
                    case "Exch":
                        _order.Exchange = items[1].ToString();
                        break;
                }
            });

            //Console.WriteLine(Thread.CurrentThread.Name + " Order: {Name - " + _order.Name + ", Ask: " + _order.Ask + ", AskSize: " + _order.AskSize
              //                + "Bid: " + _order.Bid + ", BidSize: " + _order.BidSize + ", Last: " + _order.Last + ", Exch: " + _order.Exchange + "}");

            if(cache.Contains(_order.Name + _order.Exchange)) {
                //Console.WriteLine("1");
                // upadte the ask and bid as you recieved
                Order tmp = (Order)cache.Get(_order.Name + _order.Exchange);
                if(_order.Ask > 0) 
                {
                    tmp.Ask = _order.Ask;
                    tmp.AskSize = _order.AskSize;
                    tmp.Last = _order.Last;
                }

                if (_order.Bid > 0)
                {
                    tmp.Bid = _order.Bid;
                    tmp.BidSize = _order.BidSize;
                    tmp.Last = _order.Last;
                }
            } else {
                //Console.WriteLine("2");
                // add the new object
                cache.Add(new CacheItem(_order.Name + _order.Exchange, _order), policy);
            }



            if (!cache.Contains(_order.Name))
            {
                cache.Add(new CacheItem(_order.Name, new HashSet<string>()), policy);
            }

            ((HashSet<string>)cache.Get(_order.Name)).Add(_order.Exchange);

            StringBuilder builder = new StringBuilder();
            HashSet<string> exchanges = (HashSet<string>)cache.Get(_order.Name);
            foreach (string s in exchanges)
            {
                builder.Append(s + ",");
            }

            //Console.WriteLine("Registered exchanges: " + _order.Name + " - " + builder.ToString());

            calculateNBBO(_order);

        }

        private void calculateNBBO(Order order)
        {
            //Console.WriteLine("Starting NBBO clculations..... " + order.Name);

            // check if NBBO is created for this security
            lock (locker)
            {
                if (!cache.Contains("NBBO" + order.Name))
                {
                
                    NBBO nbb = new NBBO();
                    nbb.Security = order.Name;
                    cache.Add(new CacheItem("NBBO" + order.Name, nbb), policy);
                }
            }

            NBBO nbbo = (NBBO)cache.Get("NBBO" + order.Name);


            lock(nbbo)
            {
                HashSet<string> exchanges = (HashSet<string>)cache.Get(order.Name);

                List<Order> orders = new List<Order>();
                foreach (string exchange in exchanges)
                {
                    Order o = (Order)cache.Get(order.Name + exchange);
                    orders.Add(o);
                }

                if (orders.Count() == 0)
                    return;



                // find the orders having maximum bid and min ask prices
                double maxbid = orders.Max(o => o.Bid);
                double minask = orders.Min(o => o.Ask);

                IEnumerable<Order> maxo = orders.Where(o => o.Bid!=0 && o.Bid == maxbid);
                IEnumerable<Order> mino = orders.Where(o => o.Ask!=0 && o.Ask == minask);

                Console.WriteLine("Maxbid - " + maxbid + " list - " + maxo.Count());
                Console.WriteLine("Minask - " + minask + " list - " + mino.Count());

                long maxbidsize = 0;
                long minasksize = 0;

                foreach(Order o in maxo) 
                {
                    maxbidsize += o.BidSize;
                }

                foreach (Order o in mino)
                {
                    minasksize += o.AskSize;
                }

                if(maxbidsize > 0) {
                    nbbo.NbboBid = maxbid;
                    nbbo.NbboBidSize = maxbidsize;
                }

                if(minasksize > 0) {
                    nbbo.NbboAsk = minask;
                    nbbo.NbboAskSize = minasksize;
                }

                Console.WriteLine(Thread.CurrentThread.Name + "::::NBBO:::" +
                                  "Security: " + nbbo.Security + ", Bid Price: " + nbbo.NbboBid + ", Bid Size: " + nbbo.NbboBidSize +
                                  ", Ask Price: " + nbbo.NbboAsk + ", Ask Size: " + nbbo.NbboAskSize);
            }
        }

    }

}


/*


with cache1 - caching of orer is good
IBM, NYSE, ....
IBM, LSE, ....
IBM, ARD, ....

AAPL, NYSE, ....
AAPL, LSE, ....


GET(security)
cache 2 solves this problem

*/