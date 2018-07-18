using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ContinuousPipeline
{
    class Program
    {
        public static BufferBlock<InputClass> bufferBlock;
        public static ActionBlock<InputClass> outputBlock;
        public static TransformBlock<InputClass, InputClass> transformBlock;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Run().Wait();
            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        public static async Task Run()
        {
            BuildPipeline();
            InputClass inputClass = new InputClass
            {
                inputString = "string input",
                transformString = "",
                Completion = new TaskCompletionSource<string>()
            };
            bufferBlock.Post(inputClass);

            var test = inputClass.Completion.Task;
            Console.WriteLine("After Pipeline: {0}",test.Result);
        }

        public static void BuildPipeline()
        {
            bufferBlock = new BufferBlock<InputClass>();

            transformBlock = new TransformBlock<InputClass, InputClass>(input =>
            {
                input.transformString = "Transformed at " + DateTime.Now;
                return input;
            });

            outputBlock = new ActionBlock<InputClass>(input =>
            {
                Console.WriteLine("Output: {0}", input.transformString);
                input.Completion.SetResult(input.transformString);
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            bufferBlock.LinkTo(transformBlock, options);
            transformBlock.LinkTo(outputBlock, options);
        }

    }

    public class InputClass
    {
        public string inputString { get; set; }
        public string transformString { get; set; }
        
        public TaskCompletionSource<string> Completion { get; set; }
    }
}
