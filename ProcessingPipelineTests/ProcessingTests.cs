using ProcessingPrototype;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProcessingPipelineTests
{
    public class ProcessingTests
    {
        [Fact]
        public async Task TestJsonToAppraisalsTransform()
        {
            string FilePath = ".\\EntryInformation.json";
            Processing processing = new Processing();
            string input = await processing.ReadJson(FilePath);
            Appraisals appraisals = processing.TransformIntoObjects(input);

            Assert.NotNull(appraisals);
        }
    }
}
