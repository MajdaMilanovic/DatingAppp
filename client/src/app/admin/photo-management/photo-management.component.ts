import { Component, inject, OnInit } from '@angular/core';
import { Photo } from '../../_models/photo';
import { AdminService } from '../../_services/admin.service';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css'
})
export class PhotoManagementComponent implements OnInit {
  ngOnInit(): void {
   this.getPhotosForApproval();
  }
  photos: Photo[] = [];
  adminService = inject(AdminService);
  private toastr = inject(ToastrService);
  tagFilter = '';

  loadPhotosByTags() {
    const tagList = this.tagFilter

      .split(',')

      .map((tag) => tag.trim())

      .filter((tag) => tag.length > 0);

    if (tagList.length === 0) {
      this.getPhotosForApproval();

      return;
    }

    this.adminService.getPhotosByTags(tagList).subscribe({
      next: (photos) => {
        this.photos = photos;
      },

      error: (err) => {
        console.log(err);

        this.toastr.error("Couldn't filter photos by tags");
      },
    });
  }

  getPhotosForApproval() 
  {
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => this.photos = photos
    })
  }

  approvePhoto(photoId: number) 
  {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        const photoToApprove = this.photos.find((p) => p.id === photoId);
        if (photoToApprove) {
          this.photos = this.photos.filter((p) => p.id !== photoId);
        }
      },
      error: (err) => {
        console.log(err);
        this.toastr.error('Issue with approving photo');
      },
    });
  }

  rejectPhoto(photoId: number) 
  {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        const photoToReject = this.photos.find((p) => p.id === photoId);
        if (photoToReject) {
          this.photos = this.photos.filter((p) => p.id !== photoId);
        }
      },
      error: (err) => {
        console.log(err);
        this.toastr.error('Issue with rejecting photo');
      },
    });
  }
}
