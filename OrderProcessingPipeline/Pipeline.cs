using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace OrderProcessingPipeline
{
    public class Pipeline
    {
        TransformBlock<Order, Order> receiveData;
        BroadcastBlock<Order> broadcastOrder;
        TransformBlock<Order, InvolvedPartyTransforms> processInvolvedParties;
        TransformBlock<Order, OrderTransform> processOrder;
        TransformBlock<Order, DocumentTransform> processDocuments;
        TransformBlock<Tuple<DocumentTransform, InvolvedPartyTransforms, OrderTransform>,  Order> compileInformation;
        TransformBlock<Order, Order> processOther;
        TransformBlock<Order, Order> executeTransaction;

        CancellationTokenSource cancellationTokenSource;

        public Pipeline()
        { }

        /// <summary>
        ///                              Recieve Data
        ///                                    |
        ///                                Broadcast
        ///                  __________________|_________________
        ///              Document      Involved Parties        Order
        ///                 |__________________|_________________|
        ///                                    |
        ///                            Compile InformationS
        /// </summary>
        public void StartPipeline()
        {
            ////TODO Need to add Buffer block to recieve batch post, then fill pipeline
            //Broadcasts the received data 
            broadcastOrder = new BroadcastBlock<Order>(order => order);
            //Joins all the parallel processes together to output for compiling
            var joinblock = new JoinBlock<DocumentTransform, InvolvedPartyTransforms, OrderTransform>(new GroupingDataflowBlockOptions { Greedy = false });
            
            //Add cancelation to the pipeline
            cancellationTokenSource = new CancellationTokenSource();

            ExecutionDataflowBlockOptions executionDataflowBlockOptions = new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationTokenSource.Token
            };

            //Receives input orders
            receiveData = new TransformBlock<Order, Order>(order =>
            {
                return order != null ? order : null;
            }, executionDataflowBlockOptions);

            //Processes Client/Lender information
            processInvolvedParties = new TransformBlock<Order, InvolvedPartyTransforms>(order =>
            {

                if (ClientVerification.VerifyClient(order.clientInformation).Result && ClientVerification.VerifyLender(order.lenderInformation).Result)
                {
                    ClientTransform clientTransform = new ClientTransform
                    {
                        ClientInformation = order.clientInformation
                    };
                    LenderTransform lenderTransform = new LenderTransform
                    {
                        lenderInformation = order.lenderInformation
                    };
                    InvolvedPartyTransforms involvedPartyTransforms = new InvolvedPartyTransforms
                    {
                        lenderTransform = lenderTransform,
                        clientTransform = clientTransform
                    };
                    return involvedPartyTransforms;
                }
                return null;
            }, executionDataflowBlockOptions);
            
            //Processes Order Information
            processOrder = new TransformBlock<Order, OrderTransform>(order =>
            {
                OrderTransform orderTransform = new OrderTransform
                {
                    order = order
                };
                return orderTransform;
            }, executionDataflowBlockOptions);

            //TODO add more processing
            //Processes Documents from order
            processDocuments = new TransformBlock<Order, DocumentTransform>(order =>
            {
                List<Document> documents = order.documents;
                //if(documents.Count < 0)
                //{ return null; }

                //foreach(Document document in documents)
                //{
                //    if(document == null)
                //    { return null; }
                //}
                //TODO generate events
                DocumentTransform documentTransform = new DocumentTransform
                {
                    documents = documents
                };
                return documentTransform;
            }, executionDataflowBlockOptions);

            //Compiles the information back after processing
            compileInformation = new TransformBlock<Tuple<DocumentTransform, InvolvedPartyTransforms, OrderTransform>, Order>((transforms) =>
            {
                Console.WriteLine("Compiling Order #{0}", transforms.Item3.order.orderId);
                Console.WriteLine("Lender: {0} | Client: {1}", 
                    transforms.Item2.lenderTransform.lenderInformation.lenderName,
                    transforms.Item2.clientTransform.ClientInformation.clientFName + ' ' +
                    transforms.Item2.clientTransform.ClientInformation.clientLName);
                return transforms.Item3.order;
            }, executionDataflowBlockOptions);

            //Linking options
            var options = new DataflowLinkOptions { PropagateCompletion = true};

            receiveData.LinkTo(broadcastOrder);
            //Broadcasts received data to processing blocks
            broadcastOrder.LinkTo(processInvolvedParties);
            broadcastOrder.LinkTo(processOrder);
            broadcastOrder.LinkTo(processDocuments);

            processDocuments.LinkTo(joinblock.Target1, options);
            processInvolvedParties.LinkTo(joinblock.Target2, options);
            processOrder.LinkTo(joinblock.Target3, options);
            joinblock.LinkTo(compileInformation, options);

            //Creates random orders and posts them to the Pipeline Batchblock
            //for (int i = 0; i < 50; i++)
            //{
            //    Random random = new Random();

            //    Order order = new Order().RandomOrder();

            //}

            //Currently is just manually loading two orders
            Order orderRand = new Order().RandomOrder();

            var anotherOrder = new Order().RandomOrder();

            receiveData.Post(orderRand);
            receiveData.Post(anotherOrder);

            receiveData.Complete();


            const int TIMEOUT = 30000;
            //compileInformation.Completion.Wait(TIMEOUT);

            //Currently is just manually receiving two orders
            var test = compileInformation.ReceiveAsync(cancellationTokenSource.Token);

            Console.WriteLine("One: " + test.Result.orderId);

            test = compileInformation.ReceiveAsync(cancellationTokenSource.Token);

            Console.WriteLine("Two: " + test.Result.orderId);
        }
    }
}
