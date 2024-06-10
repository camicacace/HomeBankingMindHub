using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Servicies
{
    public interface ICardService
    {
        public string FormatDebitCardNumber(string number);
        public string FormatCreditCardNumber(string number);
        public string UniqueCardNumber(string type);
        public int GenerateCVV();
        public bool CardExists(long idClient, string type, string color);
        public IEnumerable<Card> getClientsCards(long idClient);
        public IEnumerable<Card> getCardsByType(long idClient, string type);
        public void SaveCard(Card card);
    }
}
