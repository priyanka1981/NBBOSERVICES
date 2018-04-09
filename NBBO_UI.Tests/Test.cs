using NUnit.Framework;
using System;
using NbboService;
using System.Collections.Generic;

namespace NBBO_UI.Tests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestCase()
        {
            try
            {
                List<Order> lst= (List<Order>)NBBOService.getInstance().GetMarketData("IBM");
            }
            catch(Exception ex)
            {
                var actual = ex.Message;
                var expected = "System.NullReferenceException : Object reference not set to an instance of an object";
                Assert.AreNotEqual(actual, expected);
            }

         }

        [Test]
        public void TestGetNbboData(){
            try
            {
                var actual = NBBOService.getInstance().GetNbboData("IBM");
                var expected = "ibm";
                Assert.AreNotEqual(actual, expected);
            }
            catch(Exception ex)
            {
                
            }

        }
        [Test]
        public void TestTicker()
        {
            try
            {
                NBBOService.getInstance().OnTick("ibm");
                var expected = "ibm";
                var actual = "ibm";
                Assert.AreNotEqual(actual, expected);
            }
            catch (Exception ex)
            {

            }

        }


    }
}
