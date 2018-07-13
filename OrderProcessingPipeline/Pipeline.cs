using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace OrderProcessingPipeline
{
    class Pipeline
    {
        TransformBlock<Order, Order> receiveData;
        BroadcastBlock<Order> broadcastOrder;
        //All Processing Blocks return types will change
        //As they will be procesing individual parts of the Order
        //Then returning all the processed data back to the compile block
        TransformBlock<Order, Order> processClient;
        TransformBlock<Order, Order> processLender;
        TransformBlock<Order, Order> processOrder;
        TransformBlock<Order, Order> processDocuments;

        TransformBlock<Order, Order> compileInformation;
        TransformBlock<Order, Order> processOther;
        TransformBlock<Order, Order> executeTransaction;

    }
}
