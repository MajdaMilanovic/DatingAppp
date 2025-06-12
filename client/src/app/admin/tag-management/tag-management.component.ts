import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { NgFor } from '@angular/common';
import { Tag } from '../../_models/tag';

@Component({
  selector: 'app-tag-management',
  standalone: true,
  imports: [FormsModule, NgFor],
  templateUrl: './tag-management.component.html',
  styleUrl: './tag-management.component.css',
})
export class TagManagementComponent implements OnInit {
  tags: Tag[] = [];
  newTag: string = '';

  constructor(
    public adminService: AdminService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadTags();
  }

  loadTags() {
    this.adminService.getAllTags().subscribe({
      next: (tags) => (this.tags = tags),
      error: (err) => {
        console.error(err);
        this.toastr.error("Couldn't load tags");
      },
    });
  }

  addTag() {
    const name = this.newTag.trim();

    if (!name) return;

    this.adminService.addTag({ name }).subscribe({
      next: (tag) => {
        this.tags.push(tag);
        this.newTag = '';
        this.toastr.success('Tag added');
      },

      error: (err) => {
        console.error(err);
        this.toastr.error('Failed to add tag');
      },
    });
  }

  deleteTag(tagId: number) {
    this.adminService.deleteTag(tagId).subscribe({
      next: () => {
        this.tags = this.tags.filter((t) => t.id !== tagId);
        this.toastr.success('Tag deleted');
      },

      error: (err) => {
        console.error(err);
        this.toastr.error('Failed to delete tag');
      },
    });
  }
}
