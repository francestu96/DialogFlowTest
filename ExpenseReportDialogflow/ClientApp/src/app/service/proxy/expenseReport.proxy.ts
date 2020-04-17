import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MessageModel } from 'src/app/models/messageModel';

export class DialogflowResponse {
  responseText: string;
}

/// Any parameter specified here is fully optional.
/// Dialogflow will personally ask the user to fill in any missing parameters.
export interface DialogflowQueryOptions {
  /// The city that the user is sending this request from (ex. Genoa).
  userGeoCity?: string;
}

@Injectable({ providedIn: 'root' })
export class ExpenseReportProxy {
  private apiBaseUrl = 'http://localhost:8093';

  constructor(private httpClient: HttpClient) {}

  getReply(model: MessageModel): Observable<DialogflowResponse> {
    const postOptions = { headers: new HttpHeaders().set('Content-Type', 'application/json').set('Accept', 'application/json') };
    const endpoint = `${this.apiBaseUrl}/Debug/`;

    const post = this.httpClient.post<DialogflowResponse>(endpoint, JSON.stringify(model), postOptions);
    return post.pipe();
  }
}
