import { CanActivateFn, Router } from '@angular/router';

import { inject } from '@angular/core';
import { AuthStoreService } from '../_services/auth-store.service';
import { map, take } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const authStore = inject(AuthStoreService);
  const router = inject(Router);

  return authStore.currentUser$.pipe(
    take(1),
    map((user) => {
      if (!user) return router.createUrlTree(['/login']);
      const decoded = JSON.parse(atob(user.token.split('.')[1]));
      const roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];
      return roles.includes('Admin') || roles.includes('Moderator')
        ? true
        : router.createUrlTree(['/access-denied']);
    })
  );
};
