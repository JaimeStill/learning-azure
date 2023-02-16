import {
  Component, Inject, OnDestroy, OnInit
} from '@angular/core';

import {
  MsalService, MsalBroadcastService,
  MSAL_GUARD_CONFIG, MsalGuardConfiguration
} from '@azure/msal-angular';

import {
  AuthenticationResult, EventMessage,
  EventType, InteractionStatus, InteractionType,
  PopupRequest, RedirectRequest
} from '@azure/msal-browser';

import {
  filter, takeUntil
} from 'rxjs/operators';

import { Subject } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  private readonly destroying$ = new Subject<void>();

  title = 'Microsoft Identity Platform';
  isIframe = false;
  loginDisplay = false;

  constructor(
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private authService: MsalService,
    private broadcastService: MsalBroadcastService
  ) { }

  setLoginDisplay = () =>
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;

  checkAndSetActiveAccount = () => {
    const activeAccount = this.authService.instance.getActiveAccount();

    if (!activeAccount && this.authService.instance.getAllAccounts().length > 0) {
      this.authService.instance.setActiveAccount(
        this.authService.instance.getAllAccounts()[0]
      );
    }
  }

  ngOnInit(): void {
    this.isIframe = window !== window.parent && !window.opener;
    this.setLoginDisplay();

    this.authService.instance.enableAccountStorageEvents();

    this.broadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.ACCOUNT_ADDED || msg.eventType === EventType.ACCOUNT_REMOVED)
      )
      .subscribe(() =>
        this.authService.instance.getAllAccounts().length === 0
          ? window.location.pathname = "/"
          : this.setLoginDisplay()
      );

    this.broadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this.destroying$)
      )
      .subscribe(() => {
        this.setLoginDisplay();
        this.checkAndSetActiveAccount();
      })
  }

  ngOnDestroy(): void {
    this.destroying$.next(undefined);
    this.destroying$.complete();
  }

  login() {
    if (this.msalGuardConfig.interactionType === InteractionType.Popup)
      this.msalGuardConfig.authRequest
        ? this.authService.loginPopup({ ...this.msalGuardConfig.authRequest } as PopupRequest)
            .subscribe((response: AuthenticationResult) =>
              this.authService.instance.setActiveAccount(response.account)
            )
        : this.authService.loginPopup()
            .subscribe((response: AuthenticationResult) =>
              this.authService.instance.setActiveAccount(response.account)
            )
    else
      this.msalGuardConfig.authRequest
        ? this.authService.loginRedirect({ ...this.msalGuardConfig.authRequest } as RedirectRequest)
        : this.authService.loginRedirect();
  }

  logout() {
    const account = this.authService.instance.getActiveAccount()
      || this.authService.instance.getAllAccounts()[0];

    if (this.msalGuardConfig.interactionType === InteractionType.Popup)
      this.authService.logoutPopup({
        account
      })
    else
      this.authService.logoutRedirect({
        account
      })
  }
}
