using HomeBankingMindHub.Models;
using System;

namespace HomeBankingMindHub
{
    public class Utils
    {

        public static string GenerateAccountNumber()
        {
            Random random = new Random();
            int randomInt = random.Next(0, 100000000);
            string randomEightDigitNumber = randomInt.ToString("D8");
            return "VIN" + randomEightDigitNumber;
        }

        public static string UniqueCardNumber(string type)
        {

                Random random = new Random();
                long part1 = random.Next(1000000000);
                long part2 = random.Next(1000000000);

                // Combina las dos partes y toma los primeros 16 dígitos
                var randomNumber = (part1.ToString("D9") + part2.ToString("D9")).Substring(0, 16);


            if (type == "DEBIT")
            {
                randomNumber = FormatDebitCardNumber(randomNumber);
            }
            else if (type == "CREDIT")
            {
                randomNumber = FormatCreditCardNumber(randomNumber);
            }

            return randomNumber;
        }

        public static string FormatDebitCardNumber(string number)
        {
            return string.Join("-", number.Substring(0, 4), number.Substring(4, 4), number.Substring(8, 4), number.Substring(12, 4));
        }

        public static string FormatCreditCardNumber(string number)
        {
            return string.Join("-", number.Substring(0, 4), number.Substring(4, 4), number.Substring(8, 3), number.Substring(11, 4));
        }

        public static int GenerateCVV()
        {
            Random random = new Random();
            return random.Next(100, 999);
        }
    }
}
