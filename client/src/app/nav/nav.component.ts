import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';
import { AuthStoreService } from '../_services/auth-store.service';
import { NgIf } from '@angular/common';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [
    FormsModule,
    BsDropdownModule,
    RouterLink,
    RouterLinkActive,
    HasRoleDirective,
    NgIf,
    AsyncPipe,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  model: any = {};
  isLoggedIn$ = this.authStore.isLoggedIn$;
  currentUser$ = this.authStore.currentUser$;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private authStore: AuthStoreService
  ) {}

  login() {
    this.authStore.login(this.model).subscribe({
      next: (_) => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => this.toastr.error(error.error),
    });
  }
  logout() {
    this.authStore.logout();
    this.router.navigateByUrl('/');
  }
}
