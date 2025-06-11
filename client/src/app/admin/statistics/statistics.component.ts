import { NgFor } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { PhotoStats } from '../../_models/photostat';
import { UserWithoutMainPhoto } from '../../_models/userWithoutMainPhoto';
import { AdminService } from '../../_services/admin.service';

@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [NgFor],
  templateUrl: './statistics.component.html',
  styleUrl: './statistics.component.css'
})
export class StatisticsComponent implements OnInit{

   
    photoStats: PhotoStats[] = [];
    usersWithoutMainPhoto: UserWithoutMainPhoto[] = [];


    constructor(private adminService:AdminService){

  }
  ngOnInit(): void {
   
    this.adminService.getPhotoApprovalStats().subscribe(data => this.photoStats = data);
    this.adminService.getUsersWithoutMainPhoto().subscribe(data => this.usersWithoutMainPhoto = data);

  }
}
