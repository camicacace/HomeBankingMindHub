namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
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
        }
    }
}
