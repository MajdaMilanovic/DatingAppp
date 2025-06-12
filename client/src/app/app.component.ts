import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from './nav/nav.component';
import { AccountService } from './_services/account.service';
import { LoadingService } from './_services/loading.service';
import { AuthStoreService } from './_services/auth-store.service';
import { LoadingSpinnerComponent } from './loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavComponent, LoadingSpinnerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  constructor(
    public accountService: AccountService,
    public loadingService: LoadingService,
    public authStoreService: AuthStoreService,
    public cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.setCurrentUser();

    setTimeout(() => {
      this.cdRef.detectChanges();
    });
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}
