using Newtonsoft.Json;
using PipelineServicePrototype.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PipelineServicePrototype.Processing
{
    public static class JsonPipelineProcessing
    {
        private static string strPath = Environment.GetFolderPath(
                 System.Environment.SpecialFolder.DesktopDirectory);

        public static Response GenerateJson(Order order)
        {
            string fileName = Path.GetRandomFileName().Replace(".", string.Empty);
            string fullPath = Path.Combine(strPath, fileName + ".txt");

            using (StreamWriter file = File.CreateText(fullPath))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(file, order);

            }
            var response = new Response
            {
                Type = typeof(Order).ToString(),
            };

            if (File.Exists(fullPath))
            {
                response.Status = "Success";
                response.ProccessedUnits = 1;
            }

            return response;
        }
    }
}
