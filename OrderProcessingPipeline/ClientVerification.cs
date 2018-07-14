using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
