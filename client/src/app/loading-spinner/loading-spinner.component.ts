import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { LoadingService } from '../_services/loading.service';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-spinner.component.html',
  styleUrl: './loading-spinner.component.css'
})
export class LoadingSpinnerComponent {
loadingService = inject(LoadingService);
}
