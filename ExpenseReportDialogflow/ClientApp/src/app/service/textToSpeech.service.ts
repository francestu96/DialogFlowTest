import { Injectable } from '@angular/core';
import { TextToSpeech } from '@ionic-native/text-to-speech/ngx';

export interface SpeechToTextOptions {
  language: string;
  matches: number;
  prompt: string;
  showPopup: boolean;
  showPartial: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TextToSpeechService {
  constructor(private textToSpeech: TextToSpeech) {
  }

  speak(msg: string): void {
    this.textToSpeech.speak({
      text: msg,
      locale: 'it-IT'
    });
    // TODO Errors
  }
}
