import { Injectable } from '@angular/core';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { Photo } from '../_models/photo';
import { AdminService } from './admin.service';

@Injectable({
  providedIn: 'root'
})
export class PhotoFilterService {

  constructor(private adminService: AdminService) { }

  private tagFilterSubject = new BehaviorSubject<string[]>([]);
  tagFilter$ = this.tagFilterSubject.asObservable();
 
  private rawPhotosSubject = new BehaviorSubject<Photo[]>([]);
  rawPhotos$ = this.rawPhotosSubject.asObservable();
 
  filteredPhotos$ = combineLatest([this.rawPhotos$, this.tagFilter$]).pipe(
    map(([photos, tags]) => {
      if (!tags.length) return photos;
      return photos.filter((photo) =>
        photo.tags?.some((tag) => tags.includes(tag.name.toLowerCase()))
      );
    })
  );
   setPhotosForCurrentView(photos: Photo[]) {
    this.rawPhotosSubject.next(photos);
  }
 
  loadPhotos() {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photos) => this.rawPhotosSubject.next(photos),
    });
  }
 
  setTagFilter(tags: string[]) {
    this.tagFilterSubject.next(tags.map((t) => t.trim().toLowerCase()));
  }
 
  clearFilter() {
    this.tagFilterSubject.next([]);
  }
 
  removePhoto(photoId: number) {
    const current = this.rawPhotosSubject.getValue();
    this.rawPhotosSubject.next(current.filter((p) => p.id !== photoId));
  }
}
