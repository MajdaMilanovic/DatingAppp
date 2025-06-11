import { Component } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagementComponent } from "../user-management/user-management.component";
import { HasRoleDirective } from '../../_directives/has-role.directive';
import { PhotoManagementComponent } from "../photo-management/photo-management.component";
import { TagManagementComponent } from "../tag-management/tag-management.component";
import { StatisticsComponent } from '../statistics/statistics.component';
import { ApprovedFeedComponent } from "../approved-feed/approved-feed.component";


@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [TabsModule, UserManagementComponent, HasRoleDirective, PhotoManagementComponent, TagManagementComponent, StatisticsComponent, ApprovedFeedComponent],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent {

}
