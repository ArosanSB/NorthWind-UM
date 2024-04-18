using BenchmarkDotNet.Attributes;
using NorthWind_UM.Models;

namespace NorthWind_UM.Benchmark
{
    public class Benchmark1
    {
        [Benchmark]
        public void Execute()
        {
            NorthwindContext context = new NorthwindContext();
            foreach (Customer customer in context.Customers)
            {
                Console.WriteLine(customer.ContactName);
            }
        }
    }
}
