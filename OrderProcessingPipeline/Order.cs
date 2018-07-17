using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessingPipeline
{
    public enum OrderType
    {
        Purchase,
        Refinance,
        Sell
    }
    public enum EmployementStatus
    {
        Unemployed,
        Student,
        Retired,
        FullTimePrivate,
        FullTimePublic,
        SelfEmployed
    }

    public enum DocumentType
    {
        ProofOfResidence,
        ProofOfIdentity,
        SupportingDocument,
        Other
    }

    public enum OrderStatus
    {
        Initial,
        Processing,
        Verifying,
        Sent,
        Complete,
        Invalid,
        Rejected
    }
    /// <summary>
    /// Order Object contains all information for a newly created order
    /// </summary>
    public class Order
    {
        public Int32 orderId { get; private set; }
        public OrderType orderType { get; set; }
        public OrderStatus orderStatus { get; set; }
        public DateTime createdDate { get; private set; }

        public ClientInformation clientInformation { get; set; }
        public LenderInformation lenderInformation { get; set; }
        public Other other { get; set; }

        public List<Document> documents { get; set; }

        //Need more order details

        public Order()
        {
            CreateOrderId();
            createdDate = DateTime.Now;
        }

        public Order RandomOrder()
        {
            const int NAMES = 5;
            string[] firstNames = { "John", "Jane", "Joe", "Jen", "Bob", "Beth" };
            string[] lastNames = { "Smith", "Stone", "Doe", "Miller", "Davis", "Wilson" };

            Random random = new Random();
            orderType = (OrderType)(random.Next() % Enum.GetNames(typeof(OrderType)).Length);
            orderStatus = OrderStatus.Initial;

            clientInformation = new ClientInformation {
                clientAddressOne = random.Next(100, 5000).ToString() + " Main St",
                clientAddressTwo = "Apt " + random.Next(100, 700).ToString(),
                clientFName = firstNames[random.Next(NAMES) ],
                clientLName = lastNames[random.Next(NAMES)],
                clientCreditScore = random.Next(550, 850),
                clientSSN = random.Next(100000000, 999999999),
                clientState = "PA",
                clientZip = random.Next(10000,99999),
                employementStatus = (EmployementStatus)(random.Next() % Enum.GetNames(typeof(EmployementStatus)).Length)
            };

            lenderInformation = new LenderInformation
            {
                lenderName = "Quicken Loans",
                lenderAddressOne = random.Next(100, 5000).ToString() + " 1st St",
                lenderAddressTwo = "Suite " + random.Next(100, 700).ToString(),
                lenderState = "MI",
                lenderZip = 48226
            };

            if(random.Next(550) % 2 == 0)
            {
                documents = new List<Document>();
                documents.Add(new Document
                {
                    documentNameHash = "DocumentHashName",
                    documentBaseSixFour = "DocumentBase64",
                    documentExtension = "pdf",
                    documentPath = random.Next(50) % 2 ==  0? ".\\Files\\TestFileOne.txt" : ".\\Files\\TestFileTwo.txt",
                    documentType = (DocumentType)(random.Next() % Enum.GetNames(typeof(DocumentType)).Length)
                });
            }
            return this;
        }
        private void CreateOrderId()
        {
            Random random = new Random();
            orderId = random.Next(100000000, 999999999);
        }

    }

    public class Document
    {
        public int documentId { get; private set; }
        public string documentPath { get; set; }
        public string documentNameHash { get; set; }
        public DocumentType documentType { get; set; }
        public string documentExtension { get; set; }
        public string documentBaseSixFour { get; set; }

        public Document()
        {
            Random random = new Random();
            documentId = random.Next(100000000, 999999999);
        }
    }

    public class ClientInformation
    {
        public Int32 clientId { get; private set; }
        public string clientFName { get; set; }
        public string clientLName { get; set; }
        public string clientAddressOne { get; set; }
        public string clientAddressTwo { get; set; }
        public string clientState { get; set; }
        public int clientZip { get; set; }
        public EmployementStatus employementStatus { get; set; }
        public int clientCreditScore { get; set; }
        public int clientSSN { get; set; }

        public ClientInformation()
        {
            Random random = new Random();
            clientId = random.Next(100000000, 999999999);
        }
    }

    public class LenderInformation
    {
        public Int32 lenderId { get; private set; }
        public string lenderName { get; set; }
        public string lenderAddressOne { get; set; }
        public string lenderAddressTwo { get; set; }
        public string lenderState { get; set; }
        public int lenderZip { get; set; }

        public LenderInformation()
        {
            Random random = new Random();
            lenderId = random.Next(100000000, 999999999);
        }
    }

    public class Other
    {
        //Other Information?
    }
}
