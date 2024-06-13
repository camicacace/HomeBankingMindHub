using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Migrations;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol;
using System.Drawing;
using System.Net.Sockets;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        private readonly PasswordHasher<Client> _passwordHasher;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _passwordHasher = new PasswordHasher<Client>();
        }

        public Response<IEnumerable<ClientDTO>> GetAllClients()
        {
            Response<IEnumerable<ClientDTO>> response = new Response<IEnumerable<ClientDTO>>();
            var clients = _clientRepository.GetAllClients();
            
            if(clients == null)
            {
                return new Response<IEnumerable<ClientDTO>>
                {
                    StatusCode = 403,
                    Message = "No clients."
                };
            }
            
                var clientsDTO = clients.Select(client => new ClientDTO(client));

            return new Response<IEnumerable<ClientDTO>>
            {
                StatusCode = 200,
                Data = clientsDTO,
            };
        }

        public Response<ClientDTO> GetById(long id)
        {
           
            var client = _clientRepository.FindById(id);
            var response = new Response<ClientDTO>();

            if (client == null)
            {
                return new Response<ClientDTO>
                {
                    StatusCode = 404,
                    Message = $"Client with ID {id} not found."
                };

            } 
                var clientDTO = new ClientDTO(client);

            return new Response<ClientDTO>
            {
                StatusCode = 200,
                Data = clientDTO,
            };
        }

        public Response<ClientDTO> GetByEmail(string email)
        {
            var client = _clientRepository.FindByEmail(email);
            var response = new Response<ClientDTO>();

            if(client == null)
            {

                return new Response<ClientDTO>
                {
                    StatusCode = 403,
                    Message = "User not found",
                };
                
            }            
                var clientDTO = new ClientDTO(client);

            return new Response<ClientDTO>
            {
                StatusCode = 200,
                Data = clientDTO
            };
        }

        public Response<IEnumerable<AccountDTO>> GetAccounts(string email) {

            var client = _clientRepository.FindByEmail(email);
            var response = new Response<IEnumerable<AccountDTO>>();
            
            if(client == null){

                return new Response<IEnumerable<AccountDTO>>
                {
                    StatusCode = 403,
                    Message = "User not found"
                };

            } 

                var clientAccounts = _accountRepository.GetAccountsByClient(client.Id);
            if (clientAccounts == null)
            {

                return new Response<IEnumerable<AccountDTO>>
                {
                    StatusCode = 400,
                    Message = "No accounts"
                };

            }
                var accountsDTO = clientAccounts.Select(a => new AccountDTO(a));

            return new Response<IEnumerable<AccountDTO>>
            {
                StatusCode = 200,
                Data = accountsDTO,
            };

        }

        public Response<ClientDTO> PostClient(NewClientDTO newClientDTO)
        {

            if (String.IsNullOrEmpty(newClientDTO.Email)
                    || String.IsNullOrEmpty(newClientDTO.Password)
                    || String.IsNullOrEmpty(newClientDTO.FirstName)
                    || String.IsNullOrEmpty(newClientDTO.LastName))
            {
                return new Response<ClientDTO>
                {
                    StatusCode = 403,
                    Message = "Missing fields"
                };
            }

            Client client = _clientRepository.FindByEmail(newClientDTO.Email);

            if (client != null)
            {

                return new Response<ClientDTO>
                {
                    StatusCode = 403,
                    Message = "Email in use"
                };
            }
            string hashedPassword = _passwordHasher.HashPassword(null, newClientDTO.Password);

            Client newClient = new Client
            {
                Email = newClientDTO.Email,
                FirstName = newClientDTO.FirstName,
                LastName = newClientDTO.LastName,
                Password = hashedPassword
            };

            _clientRepository.Save(newClient);

            var createdClient = _clientRepository.FindByEmail(newClientDTO.Email);

            Account account = new Account
            {
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = createdClient.Id,
                Number = Utils.GenerateAccountNumber(),
            };

            _accountRepository.Save(account);

            return new Response<ClientDTO>
            {
                StatusCode = 201,
                Message = "New client created",
            };
        }

        public Response<AccountDTO> PostAccount(string email)
        {
            var client = _clientRepository.FindByEmail(email);

            if (client == null)
            {

                return new Response<AccountDTO>
                {
                    StatusCode = 400,
                    Message = "User not found"
                };
            }

            if (_accountRepository.GetAccountsByClient(client.Id).Count() > 2)
            {

                return new Response<AccountDTO>
                {
                    StatusCode = 403,
                    Message = "Already has 3 accounts"
                };
            }

                var accounts = _accountRepository.GetAllAccounts();
                var accountNumber = Utils.GenerateAccountNumber();

                while (accounts.Any(acc => acc.Number == accountNumber))
                {
                    accountNumber = Utils.GenerateAccountNumber();
                }

                Account account = new Account
                {
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id,
                    Number = accountNumber,
                };

                _accountRepository.Save(account);


            return new Response<AccountDTO>
            {
                StatusCode = 201,
                Message = "Account created",
            };
        }

        public Response<CardDTO> PostCard(NewCardDTO newCardDTO, string email)
        {
            var client = _clientRepository.FindByEmail(email);
            var response = new Response<CardDTO>();

            if (client == null)
            {
                return new Response<CardDTO>
                {
                    StatusCode = 400,
                    Message = "User not found"
                };

            }

            var clientCards = _cardRepository.GetCardsByClient(client.Id);
            if (clientCards.Count() >= 6)
            {

                return new Response<CardDTO>
                {
                    StatusCode = 403,
                    Message = "No more than 6 cards allowed",
                };
            }

            newCardDTO.Color = newCardDTO.Color.ToUpper();
            newCardDTO.Type = newCardDTO.Type.ToUpper();

            if (_cardRepository.GetCardsByType(client.Id, newCardDTO.Type).Count() >= 3)
            {
                return new Response<CardDTO>
                {
                    StatusCode = 403,
                    Message = $"No more than 3 cards type {newCardDTO.Type} allowed",
                };
            }

            if (_cardRepository.ExistingCard(client.Id, newCardDTO.Type, newCardDTO.Color))
            {
                return new Response<CardDTO>
                {
                    StatusCode = 403,
                    Message = $"A card type {newCardDTO.Type} , color {newCardDTO.Color} already exists",
                };
            }


            var cards = _cardRepository.GetAllCards();
            string number = Utils.UniqueCardNumber(newCardDTO.Type);

            while (cards.Any(c => c.Number == number))
            {
                number = Utils.UniqueCardNumber(newCardDTO.Type);
            }

            Card card = new Card
            {
                ClientId = client.Id,
                CardHolder = client.FirstName + " " + client.LastName,
                Type = newCardDTO.Type,
                Color = newCardDTO.Color,
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                Number = number,
                CVV = Utils.GenerateCVV(),
            };

            _cardRepository.Save(card);

            var cardDTO = new CardDTO(card);


            return new Response<CardDTO>
            {
                StatusCode = 201,
                Message = "Card created",
                Data = cardDTO,
            };
        }

    }
}
