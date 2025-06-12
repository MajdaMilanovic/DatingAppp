import { Component, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import {
  AsyncPipe,
  DecimalPipe,
  NgClass,
  NgFor,
  NgIf,
  NgStyle,
} from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MembersService } from '../../_services/members.service';
import { Photo } from '../../_models/photo';
import { FormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PhotoFilterService } from '../../_services/photo-filter.service';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    NgStyle,
    NgClass,
    FileUploadModule,
    DecimalPipe,
    FormsModule,
    BsDropdownModule,
    AsyncPipe,
  ],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',
})
export class PhotoEditorComponent implements OnInit {
  member = input.required<Member>();
  uploader?: FileUploader;
  hasBaseDropZoneOver: boolean = false;
  baseUrl = environment.apiUrl;
  memberChange = output<Member>();
  tagInput: string = '';
  tagFilter: string = '';
  filteredPhotos: Photo[] = [];
  filteredPhotos$ = this.photoFilterService.filteredPhotos$;

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private photoFilterService: PhotoFilterService
  ) {}

  ngOnInit() {
    this.initializeUploader();
    this.photoFilterService.setPhotosForCurrentView(this.member().photos);
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: (_) => {
        const updatedMember = { ...this.member() };
        updatedMember.photos = updatedMember.photos.filter(
          (x) => x.id !== photo.id
        );
        this.memberChange.emit(updatedMember);
      },
    });
  }

  setPhotoAsMain(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: (_) => {
        const user = this.accountService.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }
        const updatedMember = { ...this.member() };
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      },
    });
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'Users/add-photo-with-tags',
      authToken: 'Bearer ' + this.accountService.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
    this.uploader.onBeforeUploadItem = (item) => {
      if (this.tagInput.trim() !== '') {
        const tagList = this.tagInput
          .split(',')
          .map((tag) => `tags=${encodeURIComponent(tag.trim())}`)
          .join('&');
        item.url = this.baseUrl + 'Users/add-photo-with-tags?' + tagList;
      } else {
        item.url = this.baseUrl + 'Users/add-photo-with-tags';
      }
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = { ...this.member() };
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);
      if (photo.isMain) {
        const user = this.accountService.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
        this.photoFilterService.setPhotosForCurrentView(updatedMember.photos);
        this.filteredPhotos = updatedMember.photos;
      }
    };
  }

  filterPhotos() {
    const tags = this.tagFilter
      .split(',')
      .map((t) => t.trim().toLowerCase())
      .filter((t) => t.length > 0);

    this.photoFilterService.setTagFilter(tags);
  }

  clearFilter() {
    this.tagFilter = '';
    this.photoFilterService.setPhotosForCurrentView(this.member().photos);
  }

  filterPhotosByTag(tagName: string) {
    this.tagFilter = tagName;
    this.filterPhotos();
  }
}
