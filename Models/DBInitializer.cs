namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context) {

            var clients = new Client[]
            {
                new Client{ FirstName = "Alejandro", LastName = "Gomez", Email = "alegomez@gmail.com", Password = "1234"},
                new Client{ FirstName = "Juana", LastName = "Lopez", Email = "jlopez@gmail.com", Password = "5678"}
            };

            context.Clients.AddRange(clients);
            context.SaveChanges();
        }
    }
}
