using Bakery.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Bakery.ImportConsole
{
  public class ImportController
  {
        
    public static IEnumerable<Product> ReadFromCsv()
    {
            string file1 = "Products.csv";
            string file2 = "OrderItems.csv";
            string[][] matrix1 = MyFile.ReadStringMatrixFromCsv(file1, true);
            string[][] matrix2 = MyFile.ReadStringMatrixFromCsv(file2, true);

            var products = matrix1
                .Select(line => new Product
                {
                    ProductNr = line[0],
                    Name = line[1],
                    Price = Convert.ToDouble(line[2])
                }).ToArray();

            var customer = matrix2
                .GroupBy(s => s[2] + ";" + s[3] + ";"+ s[4])
                .Select(line => new Customer
                {
                    CustomerNr = line.Key.Split(';')[0],
                    LastName = line.Key.Split(';')[1],
                    FirstName = line.Key.Split(';')[2]

                })
                .ToArray();

            var order = matrix2
                .GroupBy(s => s[0] + ";" + s[1] + ";" + s[2])
                .Select(line => new Order
                {
                    OrderNr = line.Key.Split(';')[0],
                    Date = DateTime.Parse(line.Key.Split(';')[1]),
                    Customer = customer.SingleOrDefault(c => c.CustomerNr == line.Key.Split(';')[2])
                }).ToArray();

            var orderItem = matrix2
                .Select(line => new OrderItem
                {
                    Amount = Convert.ToInt32(line[6]),
                    Order = order.SingleOrDefault(o => o.OrderNr == line[0]),
                    Product = products.SingleOrDefault(p => p.ProductNr == line[5])
                }).ToArray();

            foreach(var product in products)
            {
                product.OrderItems = orderItem.Where(p => p.Product.ProductNr == product.ProductNr)
                    .ToArray();
            }
            return products;
        }
    }
}


