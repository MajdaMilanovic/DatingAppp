import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileItem, FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MembersService } from '../../_services/members.service';
import { Photo, Tag } from '../../_models/photo';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgIf, NgFor, NgStyle, NgClass, FileUploadModule, DecimalPipe, FormsModule],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})


export class PhotoEditorComponent implements OnInit{
 
  constructor( private accountService: AccountService,
               private memberService: MembersService,
               private http: HttpClient,
               private toastr: ToastrService) {
  }
  member = input.required<Member>();
  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  memberChange = output<Member>();
  photoTags:Tag[] =[]; 
  selectedTags:number[]=[];
  selectedTagName:string='';
  imageId!:number;
  lastUploadedPhotoId:number | null = null;
  
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
      console.log('Uploaded photo', photo);
      this.lastUploadedPhotoId = photo.id;
      console.log('stored photo:', this.lastUploadedPhotoId);
      this.imageId = photo.id;
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

  }


    getTags() {
    this.http.get<Tag[]>(this.baseUrl + 'Tags').subscribe((data) => {
      this.photoTags = data;
    })
  }

  connectTag(tagId: number[], id:number) {
    const photoId = id;
    const payload = {tagId, photoId};
    console.log('Sending to backend:', payload);

    if(!this.selectedTags.length){
      console.log("tag");
      return;
    }
     else if( !id){
      console.log("slika");
      return;
    }
   
    this.http.post(this.baseUrl + `Tags/api/photos/${id}/tags`, tagId).subscribe({
      next: () => {
        this.toastr.success("Tag connected successfully!");
      },
      error:(err) => {
        this.toastr.error("Administrator should approve first!");
      }
    });
  }

}
