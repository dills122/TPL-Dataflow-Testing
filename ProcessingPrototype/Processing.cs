using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingPrototype
{
    public class Processing
    {
        //Tested before Pipeline is created
        public async Task Start()
        {
            string FilePath = ".\\EntryInformation.json";

            var json = await ReadJson(FilePath);

            Appraisals appraisals = TransformIntoObjects(json);

        }

        public async Task<string> ReadJson(string FilePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exection Hit: {0}", ex.Message);
                //Implement cancellation token here to abort job
                decimal d = new decimal();
            }
            return string.Empty;
        }

        public Appraisals TransformIntoObjects(string jsonInput)
        {
            Appraisals enumerable = JsonConvert.DeserializeObject<Appraisals>(jsonInput);

            return enumerable;
        }
    }
}
