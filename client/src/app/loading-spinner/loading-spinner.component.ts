import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { LoadingService } from '../_services/loading.service';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-spinner.component.html',
  styleUrl: './loading-spinner.component.css',
})
export class LoadingSpinnerComponent {
  constructor(public loadingService: LoadingService) {}
}
