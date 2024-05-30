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
        }

        //Metodo para que se autoincrementen los numeros de cuenta
        private static string GenerateNextNumber()
        {
            LastGeneratedNumber++;
            return "VIN" + LastGeneratedNumber.ToString("D3");
        }
    }
}
