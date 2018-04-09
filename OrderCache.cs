using System;
using System.Runtime.Caching;
namespace NbboService
{
    public class OrderCache
    {
        public OrderCache()
        {
        }

        //method to cache data
        public Order insert(Order o){
            return o;
        }

        public Order get(Order o){
            return o;
        }


    }
}
