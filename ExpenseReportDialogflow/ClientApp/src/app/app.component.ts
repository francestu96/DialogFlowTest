import { Component } from '@angular/core';

import { Platform } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { SpeechToTextService } from './service/speechToText.service';
import { AlertService } from './service/alert.service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss']
})
export class AppComponent {
  private diagNoStt = this.alertService.create('e-no_tts', 'Permission error', 'Cannot access Speech-to-Text services.', ['Ok']);

  constructor(
    private platform: Platform,
    private splashScreen: SplashScreen,
    private speechToText: SpeechToTextService,
    private alertService: AlertService,
    private statusBar: StatusBar
  ) {
    this.initializeApp();
  }

  initializeApp() {
    this.platform.ready().then(() => {
      this.statusBar.styleDefault();
      this.splashScreen.hide();

      this.speechToText.requestPermissions().subscribe(hasPermission => {
        if (!hasPermission){
          this.alertService.create('e-no_tts', 'Permission error', 'Cannot access Speech-to-Text services.', ['Ok'])
            // tslint:disable-next-line: no-string-literal
            .closed(_ => navigator['app'].exitApp())
            .present();
        }
      });
    });
  }
}
