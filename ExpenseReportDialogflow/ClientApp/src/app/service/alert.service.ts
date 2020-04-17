import { Injectable } from '@angular/core';
import { Subject, from } from 'rxjs';
import { AlertController } from '@ionic/angular';
import { AlertOptions } from '@ionic/core';

/// An alert identifier, used to route external logic
export type AlertId = string | number;
/// An event that originated from an Alert
export class AlertEvent<TData> {
  alertId: AlertId;
  data: TData;
  constructor(alertId: AlertId, data: TData) {
    this.alertId = alertId;
    this.data = data;
  }
}
export class AlertClosedEvent extends AlertEvent<string> {
  alertId: AlertId;
  data: string;
  get button(): string {
    return this.data;
  }
}

export class AlertPresentedEvent extends AlertEvent<any> {
  alertId: AlertId;
  data: any;
}

/**
 * Fornisce un'interfaccia Fluent per la gestione degli alert e dei loro eventi.
 * Per aprire il dialog, basta invocare `.present()`.
 */
export class AlertServiceEventBinder {
  private alertId: AlertId;
  private alert: HTMLIonAlertElement;
  private alertController: AlertController;
  private alertOptions: AlertOptions;
  public presented$: Subject<AlertPresentedEvent>;
  public closed$: Subject<AlertClosedEvent>;

  constructor(alertController: AlertController, alertId: AlertId, alertOptions: AlertOptions) {
    this.alertId = alertId;
    this.alertOptions = alertOptions;
    this.alertController = alertController;
    this.closed$ = new Subject<AlertClosedEvent>();
    this.presented$ = new Subject<AlertPresentedEvent>();
  }
  closed(callback: (evt: AlertClosedEvent) => void): AlertServiceEventBinder {
    this.closed$.subscribe((data: AlertClosedEvent) => callback(data));
    return this;
  }
  presented(callback: (evt: AlertPresentedEvent) => void): AlertServiceEventBinder {
    this.presented$.subscribe((data: AlertPresentedEvent) => callback(data));
    return this;
  }
  present(): AlertServiceEventBinder {
    this.alertController.create(this.alertOptions).then(alert => {
      this.alert = alert;
      from(this.alert.onDidDismiss()).subscribe(_ => {
        // TODO: Passare il buttonId all'AlertClosedEvent!
        this.closed$.next(new AlertClosedEvent(this.alertId, ''));
      });
      this.alert.present().then(() => {
        this.presented$.next(new AlertPresentedEvent(this.alertId, null));
      });
    });
    return this;
  }
}

@Injectable({ providedIn: 'root' })
export class AlertService {
  constructor(private alertController: AlertController) {}

  create(id: AlertId, header: string, message: string, buttons: string[]): AlertServiceEventBinder {
    return new AlertServiceEventBinder(this.alertController, id, { header, message, buttons });
  }
}
