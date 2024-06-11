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
                response.StatusCode = 403;
                response.Message = "No clients";
            }
            else
            {
                var clientsDTO = clients.Select(client => new ClientDTO(client));
                response.StatusCode = 200;
                response.Data = clientsDTO;
            }
            return response;
        }

        public Response<ClientDTO> GetById(long id)
        {
           
            var client = _clientRepository.FindById(id);
            var response = new Response<ClientDTO>();

            if (client == null)
            {
                response.StatusCode = 404;
                response.Message = $"Client with ID {id} not found.";

            } else
            {
                var clientDTO = new ClientDTO(client);
                response.StatusCode = 200;
                response.Data = clientDTO;
            }

            return response;
        }

        public Response<ClientDTO> GetByEmail(string email)
        {
            var client = _clientRepository.FindByEmail(email);
            var response = new Response<ClientDTO>();

            if(client == null)
            {
                response.StatusCode = 403;
                response.Message = "User not found";
            } else
            {
                var clientDTO = new ClientDTO(client);
                response.StatusCode = 200;
                response.Data = clientDTO;
            }

            return response;
        }

        public Response<IEnumerable<AccountDTO>> GetAccounts(string email) {

            var client = _clientRepository.FindByEmail(email);
            var response = new Response<IEnumerable<AccountDTO>>();
            
            if(client == null){
                response.StatusCode = 403;
                response.Message = "User not found";
            } else
            {

                var clientAccounts = _accountRepository.GetAccountsByClient(client.Id);
                if (clientAccounts == null)
                {
                    response.StatusCode = 400;
                    response.Message = "No accounts";
                }
                else
                {
                    var accountsDTO = clientAccounts.Select(a => new AccountDTO(a));
                    response.StatusCode = 200;
                    response.Data = accountsDTO;
                }
            }

            return response;
        }

        public Response<ClientDTO> PostClient(NewClientDTO newClientDTO)
        {
            var response = new Response<ClientDTO>();

            if (String.IsNullOrEmpty(newClientDTO.Email)
                    || String.IsNullOrEmpty(newClientDTO.Password)
                    || String.IsNullOrEmpty(newClientDTO.FirstName)
                    || String.IsNullOrEmpty(newClientDTO.LastName))
            {
                response.StatusCode = 403;
                response.Message = "Missing fields";
            }
            else
            {

                Client client = _clientRepository.FindByEmail(newClientDTO.Email);

                if (client != null)
                {
                    response.StatusCode = 403;
                    response.Message = "Email in use";
                }
                else
                {
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
                    response.StatusCode = 201;
                    response.Message = "New client created";

                }

            }
            return response;
        }

        public Response<AccountDTO> PostAccount(string email)
        {
            var client = _clientRepository.FindByEmail(email);
            var response = new Response<AccountDTO>();

            if (client == null)
            {
                response.StatusCode = 403;
                response.Message = "User not found";
            }
            else
            {
                if (_accountRepository.GetAccountsByClient(client.Id).Count() > 2)
                {
                    response.StatusCode = 403;
                    response.Message = "Already has 3 accounts";
                }
                else
                {

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
                    response.StatusCode = 201;
                    response.Message = "Account created";
                }
            }
            return response;
        }

        public Response<CardDTO> PostCard(NewCardDTO newCardDTO, string email)
        {
            var client = _clientRepository.FindByEmail(email);
            var response = new Response<CardDTO>();

            if (client == null)
            {
                response.StatusCode = 400;
                response.Message = "User not found";
            }
            else
            {
                var clientCards = _cardRepository.GetCardsByClient(client.Id);
                if (clientCards.Count() >= 6)
                {
                    response.StatusCode = 403;
                    response.Message = "No more than 6 cards allowed";
                }
                else
                {

                    newCardDTO.Color = newCardDTO.Color.ToUpper();
                    newCardDTO.Type = newCardDTO.Type.ToUpper();

                    if (_cardRepository.GetCardsByType(client.Id, newCardDTO.Type).Count() >= 3)
                    {
                        response.StatusCode = 403;
                        response.Message = $"No more than 3 cards type {newCardDTO.Type} allowed";
                    }

                    if (_cardRepository.ExistingCard(client.Id, newCardDTO.Type, newCardDTO.Color))
                    {
                        response.StatusCode = 403;
                        response.Message = $"A card type {newCardDTO.Type} , color {newCardDTO.Color} already exists";
                    }
                    else
                    {

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
                        response.StatusCode = 201;
                        response.Message = "Card created";
                        response.Data = cardDTO;
                    }
                }

            }
            return response;
        }

    }
}
