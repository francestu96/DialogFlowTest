import { Component, NgZone } from '@angular/core';
import { SpeechToTextService } from '../service/speechToText.service';
import { ExpenseReportProxy } from '../service/proxy/expenseReport.proxy';
import { TextToSpeechService } from '../service/textToSpeech.service';
import { MessageModel } from '../models/messageModel';
import { DialogFlowService } from '../service/dialogflow.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage {
  constructor(
    private speechToTextService: SpeechToTextService,
    private textToSpeechService: TextToSpeechService,
    private ngZone: NgZone,
    private expenseReportProxy: ExpenseReportProxy,
    private dialogFlowService: DialogFlowService,
  ) { }

  onStartRecording() {
    this.speechToTextService.startTranscribing().subscribe((data: string[]) => {
      this.ngZone.run(() => {
        const value = data[0];

        if (value.trim().length === 0) {
          return;
        }
        this.dialogFlowService.sendRequest();
        // this.expenseReportProxy.getReply({ text: value }).subscribe(reply => {
        //   this.textToSpeechService.speak(reply.responseText);
        // });
      });
    });
  }
}
