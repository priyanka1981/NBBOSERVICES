using System;
using System.Threading;
using System.Windows.Forms;
namespace NbboService
{
    public class NBBORunner
    {
        public NBBORunner()
        {
        }

        static PricesGenerator priceGenerator;
        static void Main(string[] args){
            System.Console.WriteLine("Please enter a ticker");

            string ticker = Console.ReadLine();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            priceGenerator = new PricesGenerator(NBBOService.getInstance());
            priceGenerator.Start();
            System.Threading.Thread.Sleep(1000); //Thread.Sleep(10000);

             

            while(true || System.Console.ReadKey() == null) {
                Console.WriteLine("Running");
                System.Threading.Thread.Sleep(1000);
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            priceGenerator.Stop();
            Console.WriteLine("I'm out of here");
        }
    }
}
