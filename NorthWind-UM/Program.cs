using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using NorthWind_UM.Benchmark;
using NorthWind_UM.Models;
using System;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//BenchmarkRunner.Run<Benchmark1>();


public class Program
{
    // Hvor mange gange skal vi køre testen
    public static int n = 30;

    // hvor mange gange skal den kører handlingen i testen.
    public static int count = 1;

    public static void Main(String[] args)
    {
        //foreach (var item in Mark4EFNote())
        //{
        //    Console.WriteLine(item.ProductName);

        //}
        //foreach (var item in Mark4SQLNote())
        //{
        //    Console.WriteLine(item.ProductName);

        //}

        Mark4EFNote();
        Mark4SQLNote();
        //Mark4EF();
        //Mark4SQL();

        //Mark5();
    }

    public static List<ProductSummary> Mark4EFNote()
    {
        List<ProductSummary> dummy = new List<ProductSummary>();
        List<double> times = new List<double>();
        double st = 0.0, sst = 0.0;

        for (int j = 0; j < n; j++)
        {
            Timer t = new Timer();
            for (int i = 0; i < count; i++)
            {
                using (var context = new NorthwindContext())
                {
                    dummy = context.Products
                        .Where(p => p.OrderDetails.Any(od => od.Order.Customer.CustomerId == "ANTON"))
                        .Select(p => new ProductSummary
                        {
                            ProductName = p.ProductName,
                            TotalQuantity = p.OrderDetails
                                            .Where(od => od.Order.Customer.CustomerId == "ANTON")
                                            .Sum(od => od.Quantity)
                        })
                        .ToList();
                }

            }
            double time = t.Check() * 1e9 / count;
            times.Add(time);
            st += time;
            Console.WriteLine("Time: {0} ns", time);
            sst += time * time;
        }

        double mean = st / n, sdev = Math.Sqrt((sst - mean * mean * n) / (n - 1));
        Console.WriteLine("{0,6:F1} +/- {1,6:F3} ns", mean, sdev);

        // Export to Text File
        string filePath = @"C:\Users\Arosan Balasingam\Desktop\Softwareudvikling\1. Semester\UndersøgelsesMetode\Rapport\NorthWind-UM\NorthWind-UM\Results\EF3.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Time (ns)");
            foreach (var time in times)
            {
                writer.WriteLine(time);
            }
            writer.WriteLine($"Mean Time: {mean} ns");
            writer.WriteLine($"Standard Deviation: {sdev} ns");
        }

        return dummy;
    }

    public static List<ProductSummary> Mark4SQLNote()
    {
        List<ProductSummary> dummy = new List<ProductSummary>();
        List<double> times = new List<double>();
        double st = 0.0, sst = 0.0;
        string connectionString = "Server=DESKTOP-GPJIGAG\\SQLEXPRESS;Database=Northwind;Integrated Security=True;Encrypt=false;TrustServerCertificate=true;";
        for (int j = 0; j < n; j++)
        {

            Timer t = new Timer();
            for (int i = 0; i < count; i++)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database");

                    string query = @"
                    SELECT 
                        ProductName, 
                        SUM(Quantity) as TOTAL 
                    FROM 
                        Products 
                    INNER JOIN 
                        [Order Details] OD ON Products.ProductID = OD.ProductID 
                    INNER JOIN 
                        Orders O ON OD.OrderID = O.OrderID 
                    INNER JOIN 
                        Customers C ON O.CustomerID = C.CustomerID 
                    WHERE 
                        C.CustomerID = 'Anton' 
                    GROUP BY 
                        ProductName;
                ";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    dummy.Clear(); // Clear previous data to store new data from this iteration
                    while (reader.Read())
                    {
                        var productSummary = new ProductSummary
                        {
                            ProductName = reader["ProductName"].ToString(),
                            TotalQuantity = Convert.ToInt32(reader["TOTAL"])
                        };
                        dummy.Add(productSummary);
                    }

                    connection.Close();
                    Console.WriteLine("Disconnected from the database");
                }

                double time = t.Check() * 1e9 / count;
                times.Add(time);
                st += time;
                Console.WriteLine("Time: {0} ns", time);
                sst += time * time;
            }
        }
            double mean = st / n, sdev = Math.Sqrt((sst - mean * mean * n) / (n - 1));
            Console.WriteLine("{0,6:F1} +/- {1,6:F3} ns", mean, sdev);

            // Export to Text File
            string filePath = @"C:\Users\Arosan Balasingam\Desktop\Softwareudvikling\1. Semester\UndersøgelsesMetode\Rapport\NorthWind-UM\NorthWind-UM\Results\SQL3.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Time (ns)");
                foreach (var time in times)
                {
                    writer.WriteLine(time);
                }
                writer.WriteLine($"Mean Time: {mean} ns");
                writer.WriteLine($"Standard Deviation: {sdev} ns");
            }

            return dummy;
        }

    //public static List<ProductSummary> Mark4EF()
    //{
    //    int n = 10;
    //    int count = 1;
    //    List<ProductSummary> dummy = new();
    //    double st = 0.0, sst = 0.0;
    //    for (int j = 0; j < n; j++)
    //    {
    //        Timer t = new Timer();
    //        for (int i = 0; i < count; i++)
    //            dummy = getCustomerOrderDetailsEF();
    //        double time = t.Check() * 1e9 / count;
    //        st += time;
    //        Console.WriteLine("Time: {0}", time);
    //        sst += time * time;
    //    }
    //    double mean = st / n, sdev = Math.Sqrt((sst - mean * mean * n) / (n - 1));
    //    Console.WriteLine("{0,6:F1} +/- {1,6:F3} ns", mean, sdev);
    //    return dummy;
    //}

        //public static List<ProductSummary> Mark4SQL()
        //{
        //    int n = 10;
        //    int count = 1;
        //    List<ProductSummary> dummy = new();
        //    double st = 0.0, sst = 0.0;
        //    for (int j = 0; j < n; j++)
        //    {
        //        Timer t = new Timer();
        //        for (int i = 0; i < count; i++)
        //            dummy = getCustomerOrderDetailsSQL();
        //        double time = t.Check() * 1e9 / count;
        //        st += time;
        //        Console.WriteLine("Time: {0}", time);
        //        sst += time * time;
        //    }
        //    double mean = st / n, sdev = Math.Sqrt((sst - mean * mean * n) / (n - 1));
        //    Console.WriteLine("{0,6:F1} +/- {1,6:F3} ns", mean, sdev);
        //    return dummy;
        //}
    public class ProductSummary
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
    }
    //public static List<ProductSummary> getCustomerOrderDetailsEF()
    //{
    //    using (var context = new NorthwindContext())
    //    {
    //        var products = context.Products
    //            .Where(p => p.OrderDetails.Any(od => od.Order.Customer.CustomerId == "ANTON"))
    //            .Select(p => new ProductSummary
    //            {
    //                ProductName = p.ProductName,
    //                TotalQuantity = p.OrderDetails
    //                                .Where(od => od.Order.Customer.CustomerId == "ANTON")
    //                                .Sum(od => od.Quantity)
    //            })
    //            .ToList();

    //        return products;
    //    }
    //}

//    public static List<ProductSummary> getCustomerOrderDetailsSQL()
//    {
//        //SqlConnection connection = ConnectToNorthwind();

//        string connectionString = "Server=DESKTOP-GPJIGAG\\SQLEXPRESS;Database=Northwind;Integrated Security=True;Encrypt=false;TrustServerCertificate=true;";
//        SqlConnection connection = new SqlConnection(connectionString);
//        connection.Open();
//        Console.WriteLine("Connected to the database");
//        List<ProductSummary> products = new List<ProductSummary>();

//        //string query = "SELECT ProductName, SUM(Quantity) as TOTAL FROM Products INNER JOIN [Order Details] OD ON Products.ProductID = OD.ProductID INNER JOIN Orders O ON OD.OrderID = O.OrderID INNER JOIN Customers C ON O.CustomerID = C.CustomerID WHERE C.CustomerID = 'Anton' GROUP BY ProductName;";
//        string query = @"
//    SELECT 
//        ProductName, 
//        SUM(Quantity) as TOTAL 
//    FROM 
//        Products 
//    INNER JOIN 
//        [Order Details] OD ON Products.ProductID = OD.ProductID 
//    INNER JOIN 
//        Orders O ON OD.OrderID = O.OrderID 
//    INNER JOIN 
//        Customers C ON O.CustomerID = C.CustomerID 
//    WHERE 
//        C.CustomerID = 'Anton' 
//    GROUP BY 
//        ProductName;
//";
//        SqlCommand command = new SqlCommand(query, connection);

//        SqlDataReader reader = command.ExecuteReader();

//        while (reader.Read())
//        {
//            var productSummary = new ProductSummary
//            {
//                ProductName = reader["ProductName"].ToString(),
//                TotalQuantity = Convert.ToInt32(reader["TOTAL"])
//            };
//            products.Add(productSummary);
//        }

//        connection.Close();
//        Console.WriteLine("Disconnected to the database");
//        //disconnectToNorthWind(connection);

//        return products;
//    }

    // make a connection to the northwind database without using entity framework
    //public static SqlConnection ConnectToNorthwind()
    //{
    //    string connectionString = "Server=DESKTOP-GPJIGAG\\SQLEXPRESS;Database=Northwind;Integrated Security=True;Encrypt=false;TrustServerCertificate=true;";
    //    SqlConnection connection = new SqlConnection(connectionString);
    //    try
    //    {
    //        connection.Open();
    //        Console.WriteLine("Connected to the database");
    //        //timerTest = new Timer();
    //        return connection;
    //    }
    //    catch (SqlException ex)
    //    {
    //        // Handle the exception (e.g., logging the error)
    //        Console.WriteLine("An error occurred while connecting to the database: " + ex.Message);
    //        return null;
    //    }
    //}


    //public static SqlConnection disconnectToNorthWind(SqlConnection connection)
    //{
    //    connection.Close();
    //    Console.WriteLine("Disconnected to the database");
    //    //double time = timerTest.Check() * 1e9 / 1;
    //    //Console.WriteLine("Database: {0}", time);

    //    return connection;

    //}

    //public static List<ProductSummary> Mark5()
    //{
    //    int n = 10, count = 1, totalCount = 0;
    //    List<ProductSummary> dummy = new();
    //    double runningTime = 0.0;
    //    do
    //    {
    //        count *= 2;
    //        double st = 0.0, sst = 0.0;
    //        for (int j = 0; j < n; j++)
    //        {
    //            Timer t = new Timer();
    //            for (int i = 0; i < count; i++)
    //                dummy = getCustomersEF();
    //            runningTime = t.Check();
    //            double time = runningTime * 1e9 / count;
    //            st += time;
    //            sst += time * time;
    //            totalCount += count;
    //        }
    //        double mean = st / n, sdev = Math.Sqrt((sst - mean * mean * n) / (n - 1));
    //        Console.WriteLine("{0,6:F1} +/- {1,8:F2} ns {2,10:D}", mean, sdev, count);
    //    } while (runningTime < 0.25 && count < Int32.MaxValue / 2);
    //    return dummy;

    //}
}