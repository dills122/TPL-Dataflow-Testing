using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingPipeline
{
    public static class ClientVerification
    {
        /// <summary>
        /// Don't we wish this was every credit check?
        /// </summary>
        /// <param name="CreditScore"></param>
        /// <returns></returns>
        public static bool CheckCreditScore(int CreditScore)
        {
            if(CreditScore >= 550 && CreditScore <=850)
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> VerifyLender(LenderInformation lenderInformation)
        {
            if(lenderInformation.lenderName == "Quicken Loans")
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> VerifyClient(ClientInformation clientInformation)
        {
            return CheckCreditScore(clientInformation.clientCreditScore);
        }
    }
}
