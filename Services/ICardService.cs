﻿namespace HomeBankingMindHub.Servicies
{
    public interface ICardService
    {
        public string FormatDebitCardNumber(string number);
        public string FormatCreditCardNumber(string number);
        public string UniqueCardNumber();
        public int GenerateCVV();
        public bool CardExists(long idClient, string type, string color);
    }
}