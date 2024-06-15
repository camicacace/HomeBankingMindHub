using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext) {}

        public bool ExistingCard(long idClient, string type, string color)
        {
            return FindByCondition(c => c.ClientId == idClient && c.Type == type && c.Color == color)
                .Any();
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll()
                .ToList();
        }

        public IEnumerable<Card> GetCardsByClient(long idClient)
        {
            return FindByCondition(c => c.ClientId == idClient)
                .ToList();
        }

        public IEnumerable<Card> GetCardsByType(long idClient, string type)
        {
            return FindByCondition(c => c.Type == type && c.ClientId == idClient)
                .ToList();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
