import { Injectable, signal } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PresenceService } from './presence.service';
import { LikesService } from './likes.service';

const LOCAL_STORAGE_KEY = 'auth_user';
@Injectable({
  providedIn: 'root',
})
export class AuthStoreService {
  private currentUserSubject = new BehaviorSubject<User | null>(
    this.getUserFromStorage()
  );
  public currentUser$: Observable<User | null> =
    this.currentUserSubject.asObservable();
  public isLoggedIn$: Observable<boolean> = this.currentUser$.pipe(
    map((user) => !!user)
  );
  likeIds = signal<number[]>([]);
  baseUrl = environment.apiUrl;
  model: any = {};

  public roles$ = this.currentUser$.pipe(
    map((user) => {
      const token = user?.token;
      if (!token) return [];
      const decoded = JSON.parse(atob(token.split('')[1]));
      const roles = decoded.role;
      return Array.isArray(roles) ? roles : [roles];
    })
  );

  constructor(
    private http: HttpClient,
    private presenceService: PresenceService,
    private likesService: LikesService
  ) {}

  getUserFromStorage(): User | null {
    const userJson = localStorage.getItem('user');
    return userJson ? JSON.parse(userJson) : null;
  }

  private saveUserToStorage(user: User | null): void {
    if (user) {
      localStorage.setItem('user', JSON.stringify(user));
    } else {
      localStorage.removeItem('user');
    }
  }

  login(model: any): Observable<User> {
    return this.http.post<User>(this.baseUrl + 'Account/login', model).pipe(
      tap((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model: any): Observable<User> {
    return this.http.post<User>(this.baseUrl + 'Account/register', model).pipe(
      tap((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    if (!user) return;
    const decodedToken = JSON.parse(atob(user.token.split('.')[1]));
    const roles = decodedToken.role;
    user.roles = Array.isArray(roles) ? roles : [roles];

    this.saveUserToStorage(user);
    this.currentUserSubject.next(user);

    this.likesService.getLikeIds().subscribe({
      next: (ids) => this.likeIds.set(ids),
      error: (err) => console.error('Failed to load likes:', err),
    });

    this.presenceService.createHubConnection(user);
  }

  logout(): void {
    this.saveUserToStorage(null);
    this.currentUserSubject.next(null);
    this.presenceService.stopHubConnection();
  }
}
