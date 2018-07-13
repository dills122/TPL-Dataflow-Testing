using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace DataFlowPipelinePrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pipeline Started at: {0}", DateTime.Now.TimeOfDay);

            FirstExample();

            Console.WriteLine("Pipeline Complete at: {0}", DateTime.Now.TimeOfDay);

            Console.WriteLine("Pipeline Batch Job Started at: {0}", DateTime.Now.TimeOfDay);

            BatchJobExample();

            Console.WriteLine("Pipeline Batch Job Complete at: {0}", DateTime.Now.TimeOfDay);

            Console.ReadKey();
        }

        public static void FirstExample()
        {
            var ReadJsonIn = new TransformBlock<string, string>(async FilePath =>
            {
                Console.WriteLine("Reading Json In From {0}", FilePath);

                return await File.ReadAllTextAsync(FilePath);
            });

            var TransformToObject = new TransformBlock<string, Appraisal>(json =>
            {
                Console.WriteLine("Transforming JSON to Object Type: {0}", typeof(Appraisal));

                return JsonConvert.DeserializeObject<Appraisal>(json);
            });

            var GenerateEvents = new ActionBlock<Appraisal>(appraisal =>
            {
                var app = (Appraisal)appraisal;

                Console.WriteLine("Client Number: {0} recieved", app.ClientId);
                Console.WriteLine("Sending Notification to Lender: {0}", app.Lender);
                Console.WriteLine("Sending Notification to Client: {0}", app.ClientName);

            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            ReadJsonIn.LinkTo(TransformToObject, linkOptions);
            TransformToObject.LinkTo(GenerateEvents, linkOptions);

            ReadJsonIn.Post(".\\ForeignSchema.json");

            ReadJsonIn.Complete();

            GenerateEvents.Completion.Wait();
        }


        public static void BatchJobExample()
        {
            const int BATCHSIZE = 10;
            const int APPRAISALS = 40;
            Random random = new Random();

            string[] firstNames = { "Tom", "Mike", "Ruth", "Bob", "John" };
            // Possible random last names.
            string[] lastNames = { "Jones", "Smith", "Johnson", "Walker" };
            var batchBlock = new BatchBlock<Appraisal>(BATCHSIZE);

            var deserializeObjects = new ActionBlock<Appraisal[]>(a =>
           {
               Console.WriteLine("Batch Block Started ");

               foreach(Appraisal appraisal in a)
               {
                   var serializedObject = JsonConvert.SerializeObject(appraisal);
                   Console.WriteLine("Serialized Object: {0}", JsonConvert.DeserializeObject<Appraisal>(serializedObject).ClientId);
               }
           });

            batchBlock.LinkTo(deserializeObjects);
            batchBlock.Completion.ContinueWith(delegate { deserializeObjects.Complete(); });
            
            for(int i = 0; i <APPRAISALS; i++)
            {
                Appraisal appraisal = new Appraisal{
                    ClientId = random.Next(10000,40000),
                    ClientAddress = random.Next(400).ToString() + "Main St ",
                    ClientName = lastNames[random.Next() % lastNames.Length] + ", " + firstNames[random.Next() % firstNames.Length],
                    ClientState = "MI",
                    ClientZip = 48226,
                    Lender = "Quiken Loans"
                };

                batchBlock.Post(appraisal);
            }

            //Marks first Block as complete
            batchBlock.Complete();
            //Waits for the final block to finish up
            deserializeObjects.Completion.Wait();

        }
    }
}
