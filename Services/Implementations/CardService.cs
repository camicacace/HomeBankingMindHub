using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Repositories;
using System;
using System.Drawing;

namespace HomeBankingMindHub.Servicies.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private Random random = new Random();
        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public string FormatDebitCardNumber(string number)
        {
            return string.Join("-", number.Substring(0, 4), number.Substring(4, 4), number.Substring(8, 4), number.Substring(12, 4));
        }

        public string FormatCreditCardNumber(string number)
        {
            return string.Join("-", number.Substring(0, 4), number.Substring(4, 4), number.Substring(8, 3), number.Substring(11, 4));
        }

        public string UniqueCardNumber(string type)
        {
            var allCards = _cardRepository.GetAllCards();
            string randomNumber;

            do
            {
                long part1 = random.Next(1000000000);
                long part2 = random.Next(1000000000);

                // Combina las dos partes y toma los primeros 16 dígitos
                randomNumber = (part1.ToString("D9") + part2.ToString("D9")).Substring(0, 16);


            } while (allCards.Any(c => c.Number == randomNumber));

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

        public int GenerateCVV()
        {
            return random.Next(100, 999);
        }

        public bool CardExists(long idClient, string type, string color)
        {
            return _cardRepository.ExistingCard(idClient,type,color);

        }
    }
}
