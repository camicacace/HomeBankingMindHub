using HomeBankingMindHub.Models.Enums;
using System;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        private static int LastGeneratedNumber = 0;

        public static void Initialize(HomeBankingContext context) {

            if (!context.Clients.Any())
           {
                var clients = new Client[]
                {
                new Client{ FirstName = "Alejandro", LastName = "Gomez", Email = "alegomez@gmail.com", Password = "1234"},
                new Client{ FirstName = "Juana", LastName = "Lopez", Email = "jlopez@gmail.com", Password = "5678"},
                new Client { FirstName = "Maria", LastName = "Rodriguez", Email = "mr@gmail.com", Password = "9012"},
                new Client { FirstName = "Lucas", LastName = "Rodriguez", Email = "lucasr@gmail.com", Password = "3456"},
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                var clientAlejandro = context.Clients.FirstOrDefault(c => c.Email == "alegomez@gmail.com");

                if (clientAlejandro != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = clientAlejandro.Id, CreationDate = DateTime.Now, Number = GenerateNextNumber(), Balance = 1000 },
                        new Account {ClientId = clientAlejandro.Id, CreationDate = DateTime.Now, Number = GenerateNextNumber(), Balance = 500 }
                    };
                    context.AddRange(accounts);
                    context.SaveChanges();
                }
            }

            if (!context.Transactions.Any())
            {
                var client1Account = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");

                if (client1Account != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction {AccountId = client1Account.Id, Type = TransactionType.CREDIT.ToString(), Amount = 5000, Description = "Transferencia recibida", Date = DateTime.Now.AddHours(-5)},
                        new Transaction {AccountId = client1Account.Id, Type = TransactionType.DEBIT.ToString(), Amount = -2000, Description = "Compra en tienda mercado libre", Date = DateTime.Now.AddHours(-6)},
                        new Transaction {AccountId = client1Account.Id, Type = TransactionType.DEBIT.ToString(), Amount = -2500, Description = "Compra en tienda xxxx", Date = DateTime.Now.AddHours(-7)}
                    };
                    context.AddRange(transactions); 
                    context.SaveChanges();
                }
            }

            if (!context.Loans.Any())
            {
                var loans = new Loan[]
               {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
               };

                context.AddRange(loans);
                context.SaveChanges();

                var client1 = context.Clients.FirstOrDefault(c => c.Email == "alegomez@gmail.com");
                if (client1 != null)
                {
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");

                    if (loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);

                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");

                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loan2.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");

                    if (loan3 != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loan3.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }
                    context.SaveChanges();
                }
            }
            if (!context.Cards.Any())
            {
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "alegomez@gmail.com");

                if (client1 != null)
                {
                    var cards = new Card[]
                    {
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT.ToString(),
                            Color = CardColor.GOLD.ToString(),
                            Number = "3325-6745-7876-4445",
                            CVV = 990,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(4),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT.ToString(),
                            Color = CardColor.TITANIUM.ToString(),
                            Number = "2234-6745-552-7888",
                            CVV = 750,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        }
                    };

                    context.AddRange(cards);
                    context.SaveChanges();

                }
            }

        }

        //Metodo para que se autoincrementen los numeros de cuenta
        private static string GenerateNextNumber()
        {
            LastGeneratedNumber++;
            return "VIN" + LastGeneratedNumber.ToString("D3");
        }
    }
}
