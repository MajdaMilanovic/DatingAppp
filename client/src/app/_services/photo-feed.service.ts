import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  distinctUntilChanged,
  interval,
  Observable,
  shareReplay,
  switchMap,
} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PhotoFeedService {
  private readonly POLL_INTERVAL = 10000;

  constructor(private http: HttpClient) {}

  getApprovedPhotos(): Observable<any[]> {
    return interval(this.POLL_INTERVAL).pipe(
      switchMap(() =>
        this.http.get<any[]>('https://localhost:5001/api/Photo/approved', {
          responseType: 'json',
        })
      ),
      distinctUntilChanged(
        (prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)
      ),
      shareReplay(1)
    );
  }
}
