using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace OrderProcessingPipeline
{
    class Pipeline
    {
        //private readonly int _batchBlock = 10;
        TransformBlock<Order, Order> receiveData;
        BroadcastBlock<Order> broadcastOrder;
        //All Processing Blocks return types will change
        //As they will be procesing individual parts of the Order
        //Then returning all the processed data back to the compile block
        TransformBlock<Order, ClientTransform> processClient;
        TransformBlock<Order, LenderTransform> processLender;
        TransformBlock<Order, Order> processOrder;
        TransformBlock<Order, DocumentTransform> processDocuments;

        TransformBlock<Tuple<DocumentTransform, ClientTransform, LenderTransform>, Order> compileInformation;
        TransformBlock<Order, Order> processOther;
        TransformBlock<Order, Order> executeTransaction;

        public Pipeline()
        { }

        /// <summary>
        /// Pipeline Start without Broadcasting
        /// No parallel processing either
        /// </summary>
        public void StartPipeline()
        {
            //var batchBlock = new BatchBlock<Order>(_batchBlock);
            ////TODO Need to add Buffer block to recieve batch post, then fill pipeline
            //Broadcasts the received data 
            broadcastOrder = new BroadcastBlock<Order>(order => order);

            //Receives input orders
            receiveData = new TransformBlock<Order, Order>(order =>
            {
                return order != null ? order : null;
            });
            //TODO More processing 
            //Processes client information
            processClient = new TransformBlock<Order, ClientTransform>(order =>
            {
                    ClientInformation clientInformation = order.clientInformation;

                if(ClientVerification.CheckCreditScore(clientInformation.clientCreditScore))
                {
                    //TODO generate events
                    ClientTransform clientTransform = new ClientTransform
                    {
                        ClientInformation = clientInformation
                    };

                    return clientTransform;
                }
                //TODO add cancelation Tokens
                return null;
            });
            //TODO more processing 
            //Processes lender information
            processLender = new TransformBlock<Order, LenderTransform>(order =>
            {
                LenderInformation lenderInformation = order.lenderInformation;
                if (lenderInformation.lenderName == "Quicken Loans")
                {
                    //TODO generate events
                    LenderTransform lenderTransform = new LenderTransform
                    {
                        lenderInformation = lenderInformation
                    };

                    return lenderTransform;
                }

                    return null;
            });
            //TODO add more processing
            //Processes Documents from order
            processDocuments = new TransformBlock<Order, DocumentTransform>(order =>
            {
                List<Document> documents = order.documents;
                if(documents.Count < 0)
                { return null; }

                foreach(Document document in documents)
                {
                    if(document == null)
                    { return null; }
                }
                //TODO generate events
                DocumentTransform documentTransform = new DocumentTransform
                {
                    documents = documents
                };
                return documentTransform;
            });

            //compileInformation = new TransformBlock<Tuple<DocumentTransform, ClientTransform, LenderTransform>, Order>((transforms) =>
            //{

            //    //TODO Order info not available here, but is needed 
            //});

            //batchBlock.LinkTo(buffer);
            receiveData.LinkTo(broadcastOrder);
            //Broadcasts received data to processing blocks
            broadcastOrder.LinkTo(processClient);
            broadcastOrder.LinkTo(processLender);
            broadcastOrder.LinkTo(processDocuments);

            //Joins all the parallel processes together to output for compiling
            var joinblock = new JoinBlock<DocumentTransform, ClientTransform, LenderTransform>(new GroupingDataflowBlockOptions { Greedy = false });

            var options = new DataflowLinkOptions { };
            processDocuments.LinkTo(joinblock.Target1, options);
            processClient.LinkTo(joinblock.Target2, options);
            processLender.LinkTo(joinblock.Target3, options);
            joinblock.LinkTo(compileInformation, options);

            //Creates random orders and posts them to the Pipeline Batchblock
            for (int i = 0; i < 50; i++)
            {
                Random random = new Random();

                Order order = new Order().RandomOrder();

               // batchBlock.Post(order);
            }

            //batchBlock.Completion.ContinueWith(delegate { buffer.Complete(); });

        }
    }
}
