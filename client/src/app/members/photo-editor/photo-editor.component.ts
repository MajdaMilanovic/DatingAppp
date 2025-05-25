import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileItem, FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MembersService } from '../../_services/members.service';
import { Photo } from '../../_models/photo';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';


    interface Tag
    {
        id:number;
        name:string;
    }

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgIf, NgFor, NgStyle, NgClass, FileUploadModule, DecimalPipe, FormsModule],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})


export class PhotoEditorComponent implements OnInit{
  private accountService = inject(AccountService);
  member = input.required<Member>();
  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  memberChange = output<Member>();
  private memberService = inject(MembersService);
  photoTags:Tag[] =[]; 
  selectedTag:number | null =null;
  selectedTagName:string='';
  private http = inject(HttpClient);
  imageId!:number;

  
  ngOnInit(): void {
    this.initializeUploader();
    this.getTags();
  }

  fileOverBase(e:any) {
    this.hasBaseDropZoneOver = e;
  }

  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: _ => {
        const updatedMember = {...this.member()};
        updatedMember.photos = updatedMember.photos.filter(x => x.id !== photo.id);
        this.memberChange.emit(updatedMember);
      }
    })
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: _ => {
        const user = this.accountService.currentUser();
        if(user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }
        const updatedMember = {...this.member()}
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach( p => {
          if(p.isMain) p.isMain = false;
          if(p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      }
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'Users/add-photo',
      authToken: 'Bearer ' + this.accountService.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10* 1024 *1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = {...this.member()}
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);
      if(photo.isMain) {
        const user = this.accountService.currentUser();
        if(user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach( p => {
          if(p.isMain) p.isMain = false;
          if(p.id === photo.id) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      }
    }
     this.uploader.onBuildItemForm = (item, form) => {
        const tags = item.formData?.tags || '';
        form.append('tags', tags);
      }

  }


    getTags() {
    this.http.get<Tag[]>(this.baseUrl + 'Tags').subscribe((data) => {
      this.photoTags = data;
    })
  }

  connectTag() {
    const selectedTag = this.photoTags.find(t =>t.id === this.selectedTag);

    if(!selectedTag){
      console.error("Tag missing");
      return;
    }

    const body = {
      tags: [selectedTag.id]
    } 

    this.http.post(this.baseUrl + `Tags/api/photos/${this.imageId}/tags`, body).subscribe({
      next: () => {
        console.log('Uspjesno povezano');
      },
      error:(err) => {
        console.log("greska", err);
      }
    });
  }

  onChange() {
    const tag = this.photoTags.find(t =>t.id === this.selectedTag);
    this.selectedTagName = tag?.name || '';
  }


}
