using BenchmarkDotNet.Attributes;
using NorthWind_UM.Models;

namespace NorthWind_UM.Benchmark
{
    public class Benchmark1
    {

        [Benchmark]
        public List<Customer> getCustomers()
        {
            NorthwindContext context = new NorthwindContext();
            List<Customer> customersExecute = new List<Customer>();
            foreach (Customer customer in context.Customers)
            {
                customersExecute.Add(customer);
               // Console.WriteLine(customer.ContactName);
            }
            return customersExecute;
        }




    }
}
