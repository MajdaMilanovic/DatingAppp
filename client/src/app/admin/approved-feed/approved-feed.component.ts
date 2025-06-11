import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { PhotoFeedService } from '../../_services/photo-feed.service';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-approved-feed',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe],
  templateUrl: './approved-feed.component.html',
  styleUrl: './approved-feed.component.css'
})
export class ApprovedFeedComponent implements OnInit {
  approvedPhotos$!: Observable<any[]>;

  constructor(private photoFeedService: PhotoFeedService) {}


  ngOnInit(): void {
    this.approvedPhotos$ = this.photoFeedService.getApprovedPhotos();
  }

  

}
