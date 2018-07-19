using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ContinuousPipeline
{
    class Program
    {
        public static BufferBlock<InputClass> bufferBlock;
        public static ActionBlock<InputClass> outputBlock;
        public static TransformBlock<InputClass, InputClass> transformBlock;
        public static List<string> processedInfo;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Run().Wait();

            List<InputClass> inputClasses = new List<InputClass>();
            for (int i = 0; i < 10; i++)
            {
                inputClasses.Add(new InputClass());
            }
            PostPipeline(inputClasses).Wait();
            foreach(string str in processedInfo)
            {
                Console.WriteLine("Recieved Posted info: {0}", str);
            }
            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        //public static async Task Run()
        //{
        //    BuildPipeline();
        //    InputClass inputClass = new InputClass
        //    {
        //        inputString = "string input",
        //        transformString = "",
        //        Completion = new TaskCompletionSource<string>()
        //    };
        //    bufferBlock.Post(inputClass);

        //    var test = inputClass.Completion.Task;
        //    Console.WriteLine("After Pipeline: {0}",test.Result);
        //}

        public static void BuildPipeline()
        {
            bufferBlock = new BufferBlock<InputClass>();

            transformBlock = new TransformBlock<InputClass, InputClass>(input =>
            {
                Console.WriteLine("Received String: {0}", input.inputString);
                input.inputString = new string(input.inputString.ToCharArray().Reverse().ToArray());
                return input;
            });

            outputBlock = new ActionBlock<InputClass>(input =>
            {
                //Console.WriteLine("Read for output: {0}", input.inputString);
                input.Completion.SetResult(input.inputString);
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            bufferBlock.LinkTo(transformBlock, options);
            transformBlock.LinkTo(outputBlock, options);
        }

        public static Task PostPipeline(List<InputClass> inputClasses)
        {
            BuildPipeline();
            processedInfo = new List<string>();
            foreach(InputClass input in inputClasses)
            {
                bufferBlock.Post(input);

                processedInfo.Add(input.Completion.Task.Result);
            }
            return Task.CompletedTask;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }

    public class InputClass
    {
        public string inputString { get; set; }
        
        public TaskCompletionSource<string> Completion { get; set; }

        public InputClass()
        {
            inputString = Program.RandomString(25);
            Completion = new TaskCompletionSource<string>();
        }
    }
}
