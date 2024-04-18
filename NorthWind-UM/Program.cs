using NorthWind_UM.Models;



NorthwindContext context = new NorthwindContext();


foreach (Customer C in context.Customers)
{
    Console.WriteLine($"Name: {C.ContactName}");
}
Console.ReadLine();
