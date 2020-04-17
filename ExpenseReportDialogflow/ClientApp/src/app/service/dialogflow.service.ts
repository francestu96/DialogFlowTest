import { Injectable } from '@angular/core';
import { dialogflow } from 'dialogflow';
import {v4 as uuid} from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class DialogFlowService {
  sendRequest(projectId = 'hotelbookingagent-vlgcgc') {
    // A unique identifier for the given session
    const sessionId = uuid.v4();
    console.log(sessionId);

    // Create a new session
    const sessionClient = new dialogflow.SessionsClient();
  //   const sessionPath = sessionClient.sessionPath(projectId, sessionId);

  //   // The text query request.
  //   const request = {
  //     session: sessionPath,
  //     queryInput: {
  //       text: {
  //         // The query to send to the dialogflow agent
  //         text: 'hello',
  //         // The language used by the client (en-US)
  //         languageCode: 'en-US',
  //       },
  //     },
  //   };

  //   // Send request and log result
  //   sessionClient.detectIntent(request).subscribe(arg => {
  //     console.log('Detected intent');
  //     const result = arg[0].queryResult;
  //     console.log(`  Query: ${result.queryText}`);
  //     console.log(`  Response: ${result.fulfillmentText}`);
  //     if (result.intent) {
  //       console.log(`  Intent: ${result.intent.displayName}`);
  //     } else {
  //       console.log(`  No intent matched.`);
  //     }
  //   });
  // }
  }
}
