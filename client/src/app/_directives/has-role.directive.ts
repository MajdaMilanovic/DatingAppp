import {
  Directive,
  inject,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AuthStoreService } from '../_services/auth-store.service';
import { take } from 'rxjs/operators';

@Directive({
  selector: '[appHasRole]',
  standalone: true,
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  private authStore = inject(AuthStoreService);

  ngOnInit() {
    this.authStore.currentUser$.pipe(take(1)).subscribe((user) => {
      if (!user) {
        this.viewContainerRef.clear();
        return;
      }
      const decodedToken = JSON.parse(atob(user.token.split('.')[1]));
      const roles = Array.isArray(decodedToken.role)
        ? decodedToken.role
        : [decodedToken.role];

      if (this.appHasRole.some((role) => roles.includes(role))) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear();
      }
    });
  }
}
