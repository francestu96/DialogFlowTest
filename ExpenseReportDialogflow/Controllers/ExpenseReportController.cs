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

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ExpenseReportController : Controller
    {
        private readonly ILogger<ExpenseReportController> Logger;
        private readonly IMapper Mapper;
        private readonly ICurrentUserService UserService;
        private IMemoryCache Cache;
        private MD5CryptoServiceProvider CryptoService;

        public ExpenseReportController(
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
        }

        [HttpPost]
        public async Task<ActionResult<string>> Query([FromBody] MessageModel message)
        {
            const string project = "expensereportagent-aeprpn";
            const string credentialsPath = "Credentials/google.json";

            if (!System.IO.File.Exists(credentialsPath))
            {
                return BadRequest("No Google credentials provided.");
            }

            var clientBuilder = new SessionsClientBuilder
            {
                CredentialsPath = credentialsPath
            };
            var client = clientBuilder.Build();
            
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
            if (!Cache.TryGetValue(userKey, out string userSession))
            {
                userSession = Guid.NewGuid().ToString();
                Cache.Set(userKey, userSession);
            }

            //entità di prova da esportare in futuro come constanti
            var ubiEntity = new SessionEntityType
            {
                Name = new SessionName(project, "us", userSession) + "/entityTypes/ExpenseType",
                EntityOverrideMode = new SessionEntityType.Types.EntityOverrideMode(),                
                //TODO: capire come assegnare la property entities che però è readonly
                //Entities = ""
            };

            var request = new DetectIntentRequest
            {
                SessionAsSessionName = new SessionName(project, "us", userSession),
                QueryParams = new QueryParameters
                {
                    //TODO: capire come assegnare la property entities che però è readonly
                    //SessionEntityTypes = ""
                },
                QueryInput = query
            };

            var dialogFlow = await client.DetectIntentAsync(
                new SessionName(project, "us", userSession),
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
