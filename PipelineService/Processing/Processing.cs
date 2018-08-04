using Newtonsoft.Json;
using PipelineService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PipelineService.Processing
{
    public static class Processing
    {
        private static string strPath = Environment.GetFolderPath(
                         System.Environment.SpecialFolder.DesktopDirectory);
        public static object ReceiveOrder(object inputObject)
        {
            var inputOrder = (Order)inputObject;
            var passValue = CheckEligibility(inputOrder.ClientCreditScore, inputOrder.LoanTotal, inputOrder.LoanDownPayment);
            if(passValue)
            {
                inputOrder.Approval = true;
                if(inputOrder.OutputType == "Xml")
                {
                    return GenerateXml(inputOrder);
                }
                else
                {
                    return GenerateJson(inputOrder);
                }
            } 
            else
            {
                object cleanObject = new Order().Reject();
                return cleanObject;
            }
        }

        public static object GenerateJson(object OrderObject)
        {
            
            Order order = (Order)OrderObject;
            string fileName =Path.GetRandomFileName();
            string fullPath = Path.Combine(strPath, fileName + ".txt");
            using (StreamWriter file = File.CreateText(fullPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, order);
                order.OutputLocation = fullPath;
            }
            return order;
        }

        public static object GenerateXml(object OrderObject)
        {
            Order order = (Order)OrderObject;
            string fileName = Path.GetRandomFileName();
            string fullPath = Path.Combine(strPath, fileName + ".txt");
            using (StreamWriter file = File.CreateText(fullPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Order));
                ser.Serialize(file, order);
                order.OutputLocation = fullPath;
            }
            return order;
        }

        public static object ReturnProcessedOrder(object OrderObject)
        {
            return OrderObject;
        }

        private static bool CheckEligibility(int creditScore, decimal LoanTotal, decimal LoanDownPayment)
        {
            if (creditScore > 650)
            {
                return CheckDownPayment(LoanTotal, LoanDownPayment);
            }
            else if (creditScore < 650 && creditScore > 500)
            {
                if(LoanTotal < 1000000)
                {
                    return CheckDownPayment(LoanTotal, LoanDownPayment);
                } 
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool CheckDownPayment(decimal LoanTotal, decimal LoanDownPayment)
        {
            var percentDown = LoanDownPayment / LoanTotal;

            if (Decimal.Round(percentDown, 2) >= (decimal).25)
            {
                return true;
            }
            else if (Decimal.Round(percentDown, 2) < (decimal).25 && Decimal.Round(percentDown, 2) >= (decimal).18)
            {
                //TODO add some logic
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
