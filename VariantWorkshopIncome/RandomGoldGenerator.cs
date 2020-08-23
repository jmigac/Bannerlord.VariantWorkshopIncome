using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariantWorkshopIncome
{
    class RandomGoldGenerator
    {
        private static Random random;
        public static int GenerateSum(int min,int max)
        {
            if (random == null)
                random = new Random();
            return random.Next(min, max);
        }
    }
}
