using Google.Cloud.Dialogflow.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseReportDialogflow.SessionEntities
{
    public static class UbiSessionEntities
    {             
        public static SessionEntityType GetEntities(SessionName sessionName)
        {
            var vittoEntity = new EntityType.Types.Entity()
            {
                Value = "vitto"
            };
            vittoEntity.Synonyms.Add(new List<string>() { "vitto", "cena", "pranzo", "colazione", "mangiato" });
            var alloggioEntity = new EntityType.Types.Entity()
            {
                Value = "alloggio"
            };
            alloggioEntity.Synonyms.Add(new List<string>() { "dormito", "alloggio" });

            var sessionEntities = new SessionEntityType
            {
                SessionEntityTypeName = new SessionEntityTypeName(sessionName.ProjectId, sessionName.SessionId, "ExpenseType"),
                EntityOverrideMode = SessionEntityType.Types.EntityOverrideMode.Override
            };
            sessionEntities.Entities.Add(new List<EntityType.Types.Entity>() { vittoEntity, alloggioEntity });

            return sessionEntities;
        }
    }
}
