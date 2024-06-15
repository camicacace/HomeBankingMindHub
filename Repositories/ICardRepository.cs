using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        public IEnumerable<Card> GetAllCards();
        public IEnumerable<Card> GetCardsByClient(long idClient);
        public IEnumerable<Card> GetCardsByType(long idClient,string type);
        public bool ExistingCard(long idClient, string type, string color);
        void Save(Card card);
    }
}
