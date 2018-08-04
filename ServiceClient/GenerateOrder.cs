using PipelineService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceClient
{

    public class GenerateOrder
    {
        public Order order { get; set; }

        public GenerateOrder(OrderType orderType)
        {
            order = new Order();
            RandomOrder(orderType);
        }

        public void RandomOrder(OrderType orderType)
        {
            const int NAMES = 5;
            string[] firstNames = { "John", "Jane", "Joe", "Jen", "Bob", "Beth" };
            string[] lastNames = { "Smith", "Stone", "Doe", "Miller", "Davis", "Wilson" };
            string[] guarantorNames = { "Sallie Mae", "Citizens Bank", "Ally Bank", "USAA", "Wells Fargo" };

            Random random = new Random();
            order.orderType = orderType;

            order.ClientFName = firstNames[random.Next(NAMES)];
            order.ClientLName = lastNames[random.Next(NAMES)];

            order.ClientCreditScore = random.Next(400, 850);

            order.GuarantorName = guarantorNames[random.Next(NAMES)];

            order.LoanTotal = random.Next(10000, 4000000);
            order.LoanDownPayment = random.Next(10000, (int)(order.LoanTotal * (decimal).50));
            order.LoanFees = order.LoanTotal * (decimal).10;
            order.LoanTotal += order.LoanFees;

            order.OutputType = random.Next(550) % 2 == 0 ? "Xml" : "Json";

            CreateOrderId();
        }
        private void CreateOrderId()
        {
            Random random = new Random();
            order.OrderId = random.Next(100000000, 999999999);
        }
    }
}
