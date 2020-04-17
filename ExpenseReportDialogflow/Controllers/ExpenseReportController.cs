using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Google.Cloud.Dialogflow.V2;
using ExpenseReportDialogflow.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Horsa.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using System;
using Google.Protobuf.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ExpenseReportDialogflow.Util;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ExpenseReportController : Controller
    {
        private static class ConfigurationKeys
        {
            public const string GoogleCredentials = "Dialogflow:Credentials";
            public const string DialogflowProject = "Dialogflow:Credentials:project_id";
        }

        private readonly ILogger<ExpenseReportController> Logger;
        private readonly IMapper Mapper;
        private readonly ICurrentUserService UserService;
        private readonly SessionsClientBuilder ClientBuilder;
        private IMemoryCache Cache;
        private MD5CryptoServiceProvider CryptoService;

        public ExpenseReportController(
            IConfiguration cfg,
            IMapper mapper, 
            ICurrentUserService userService, 
            IMemoryCache cache, 
            ILogger<ExpenseReportController> logger)
        {
            Mapper = mapper;
            UserService = userService;
            Cache = cache;
            Logger = logger;
            CryptoService = new MD5CryptoServiceProvider();

            // Get Google credentials from configuration
            var credentialsSection = cfg.GetSection(ConfigurationKeys.GoogleCredentials);
            if(!credentialsSection.Exists()) {
                throw new ArgumentException($"Google credentials were not provided. Configuration path: {ConfigurationKeys.GoogleCredentials}.");
            }
            // Cache project name from configuration
            if (cfg.GetValue<string>(ConfigurationKeys.DialogflowProject, null) is { } project) {
                Cache.Set(ConfigurationKeys.DialogflowProject, project);
            }
            else {
                throw new ArgumentException($"Dialogflow project name was not provided. Configuration path: {ConfigurationKeys.DialogflowProject}.");
            }
            // Prepare session client builder
            ClientBuilder = new SessionsClientBuilder {
                CredentialsPath = new ConfigurationSerializer(credentialsSection).ToJson(),
            };
        }

        protected SessionName GetSessionName(string userKey, string locationId)
        {
            if (!Cache.TryGetValue(userKey, out string userSession)) {
                userSession = Guid.NewGuid().ToString();
                Cache.Set(userKey, userSession);
            }
            return new SessionName(Cache.Get<string>(ConfigurationKeys.DialogflowProject), locationId, userSession);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Query([FromBody] MessageModel message)
        {
            var client = ClientBuilder.Build();
            
            var query = new QueryInput()
            {
                Text = new TextInput()
                {
                    Text = message.Text,
                    LanguageCode = "IT-it"
                }
            };

            //se l'utente non ha avuto conversazioni, creo un nuovo record nella CacheMemory
            //con chiave lo username (in realta l hash md5) e con valore la sessione (un guid casuale)
            var userKey = Encoding.ASCII.GetString(CryptoService.ComputeHash(Encoding.ASCII.GetBytes(UserService.User.Identity.Name)));
            var sessionName = GetSessionName(userKey, "us");

            //entità di prova da esportare in futuro come constanti
            var ubiEntity = new SessionEntityType
            {
                Name = sessionName + "/entityTypes/ExpenseType",
                EntityOverrideMode = new SessionEntityType.Types.EntityOverrideMode(),                
                //TODO: capire come assegnare la property entities che però è readonly
                //Entities = ""
            };

            var request = new DetectIntentRequest
            {
                SessionAsSessionName = sessionName,
                QueryParams = new QueryParameters
                {
                    //TODO: capire come assegnare la property entities che però è readonly
                    //SessionEntityTypes = ""
                },
                QueryInput = query
            };

            var dialogFlow = await client.DetectIntentAsync(
                sessionName,
                query
            );

            //se la conversazione ha risolto tutti i prametri richiesti, eseguo la insert della spesa
            if (dialogFlow.QueryResult.AllRequiredParamsPresent)
            {
                var expenseModel = Mapper.Map<ExpenseModel>(dialogFlow);
                FakeInsertExpenseReport(expenseModel);
            }

            return new ActionResult<string>(dialogFlow.QueryResult.FulfillmentText);
        }
    

        private bool FakeInsertExpenseReport(ExpenseModel model)
        {
            return true;
        }
    }
}
