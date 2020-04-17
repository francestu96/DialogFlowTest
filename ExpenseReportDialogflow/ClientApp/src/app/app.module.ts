import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouteReuseStrategy } from '@angular/router';

import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { SpeechRecognition } from '@ionic-native/speech-recognition/ngx';
import { ExpenseReportProxy } from './service/proxy/expenseReport.proxy';
import { TextToSpeech } from '@ionic-native/text-to-speech/ngx';
import { HttpClientModule } from '@angular/common/http';
import { TextToSpeechService } from './service/textToSpeech.service';
import { AlertService } from './service/alert.service';
import { SpeechToTextService } from './service/speechToText.service';
import { DialogFlowService } from './service/dialogflow.service';

@NgModule({
  declarations: [AppComponent],
  entryComponents: [],
  imports: [BrowserModule, IonicModule.forRoot(), AppRoutingModule, HttpClientModule],
  providers: [
    StatusBar,
    SplashScreen,
    SpeechToTextService,
    TextToSpeechService,
    AlertService,
    DialogFlowService,
    SpeechRecognition,
    TextToSpeech,
    ExpenseReportProxy,
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
