import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { Photo } from '../_models/photo';
import { Tag } from '../_models/tag';
import { PhotoStats } from '../_models/photostat';
import { UserWithoutMainPhoto } from '../_models/userWithoutMainPhoto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getUserWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'AdminConroller/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'AdminConroller/edit-roles/'
       + username + '?roles=' + roles, {})
  }

  getPhotosForApproval() {
    return this.http.get<Photo[]>(this.baseUrl + 'AdminConroller/photos-to-moderate');
  }
  approvePhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'AdminConroller/approvePhoto/' + photoId, {});
  }
  rejectPhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'AdminConroller/rejectPhoto/' + photoId, {});
  }

  getPhotosByTags(tags: string[]) {
    const params = new HttpParams({ fromObject: { tags } });
    return this.http.get<Photo[]>(this.baseUrl + 'photos/unapproved-by-tags', {
      params,
    });
  }

  getAllTags() {
    return this.http.get<Tag[]>(this.baseUrl + 'AdminConroller/tags');
  }

  addTag(tag: { name: string }) {
    return this.http.post<Tag>(this.baseUrl + 'AdminConroller/add-tag', tag);
  }

  deleteTag(tagId: number) {
    return this.http.delete(this.baseUrl + 'AdminConroller/delete-tag/' + tagId);
  }


  getPhotoApprovalStats(){
    return this.http.get<PhotoStats[]>(this.baseUrl + 'AdminConroller/photo-stats');
  }
   getUsersWithoutMainPhoto(){
    return this.http.get<UserWithoutMainPhoto[]>(this.baseUrl + 'AdminConroller/users-without-main-photo');
  }
}
