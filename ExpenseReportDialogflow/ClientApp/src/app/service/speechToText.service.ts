import { Injectable } from '@angular/core';
import { SpeechRecognition } from '@ionic-native/speech-recognition/ngx';
import { from, Observable, of } from 'rxjs';
import { switchMap, catchError, map } from 'rxjs/operators';

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
export class SpeechToTextService {
  static defaultOptions: SpeechToTextOptions = {
    language: 'it_IT',
    matches: 8,
    prompt: '',
    showPopup: true,
    showPartial: true
  };

  constructor(private speechRecognition: SpeechRecognition) {
  }

  isRecognitionAvailable(): Observable<boolean> {
    return from(this.speechRecognition.isRecognitionAvailable());
  }

  requestPermissions(): Observable<boolean> {
    // Check permission
    return from(this.speechRecognition.requestPermission()).pipe(
      switchMap(() => from(this.speechRecognition.hasPermission())),
      catchError(() => of(false))
    );
  }

  startTranscribing(options?: SpeechToTextOptions): Observable<string[]> {
    options = options || SpeechToTextService.defaultOptions;
    return this.speechRecognition.startListening(options);
  }
}
