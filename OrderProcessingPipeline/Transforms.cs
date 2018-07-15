using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessingPipeline
{
    public class DocumentTransform
    {
        public int TransformId { get; private set; }
        public List<Event> events { get; set; }
        public List<Document> documents { get; set; }

        public DocumentTransform()
        {
            Random random = new Random();
            TransformId = random.Next(100000, 999999);
        }
    }
    public class ClientTransform
    {
        public int TransformId { get; private set; }
        public List<Event> events { get; set; }
        public ClientInformation ClientInformation { get; set; }

        public ClientTransform()
        {
            Random random = new Random();
            TransformId = random.Next(100000, 999999);
        }
    }
    public class LenderTransform
    {
        public int TransformId { get; private set; }
        public List<Event> events { get; set; }
        public LenderInformation lenderInformation{ get; set; }

        public LenderTransform()
        {
            Random random = new Random();
            TransformId = random.Next(100000, 999999);
        }
    }
    public class InvolvedPartyTransforms
    {
        public LenderTransform lenderTransform { get; set; }
        public ClientTransform clientTransform { get; set; }
    }

    public class OrderTransform
    {
        public int TransformId { get; private set; }
        public List<Event> events { get; set; }
        public Order order { get; set; }

        public OrderTransform()
        {
            Random random = new Random();
            TransformId = random.Next(100000, 999999);
        }
    }

    public enum EventType
    {
        Client,
        Lender,
        Document
    }
    public class Event
    {
        public int eventId { get; private set; }
        public EventType eventType { get; set; }
    }
}
