using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTOs
{
    public class CardDTO
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public int CVV { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }

        public CardDTO(Card card)
        {
            Id = card.Id;
            CardHolder = card.CardHolder;
            Type = card.Type;
            Color = card.Color;
            Number = card.Number;
            CVV = card.CVV;
            FromDate = card.FromDate;
            ThruDate = card.ThruDate;
        }
    }
}
