import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatTabsModule } from '@angular/material/tabs';
import { PlanChangeRequest } from '../../../../core/models';
import { PlanChangeRequestService } from '../../../../core/services';
import { ChangeRequestDialog } from '../change-request-dialog/change-request-dialog';

@Component({
  selector: 'app-change-request-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTabsModule
  ],
  templateUrl: './change-request-list.html',
  styleUrl: './change-request-list.scss'
})
export class ChangeRequestList implements OnInit {
  private requestService = inject(PlanChangeRequestService);
  private dialog = inject(MatDialog);

  allRequests = signal<PlanChangeRequest[]>([]);
  pendingRequests = signal<PlanChangeRequest[]>([]);
  loading = signal(true);
  displayedColumns = ['contractNumber', 'fromPlan', 'toPlan', 'requestedBy', 'requestedAt', 'status', 'actions'];

  ngOnInit() {
    this.loadRequests();
  }

  loadRequests() {
    this.loading.set(true);

    // Load all requests
    this.requestService.getAll().subscribe({
      next: (requests) => {
        this.allRequests.set(requests);
        this.pendingRequests.set(requests.filter(r => r.status === 'Pending'));
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending':
        return 'accent';
      case 'Approved':
        return 'primary';
      case 'Rejected':
        return 'warn';
      case 'Cancelled':
        return '';
      default:
        return '';
    }
  }

  processRequest(request: PlanChangeRequest) {
    const dialogRef = this.dialog.open(ChangeRequestDialog, {
      width: '500px',
      data: request
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadRequests();
      }
    });
  }

  cancelRequest(request: PlanChangeRequest, event: Event) {
    event.stopPropagation();
    if (confirm(`Cancel change request for ${request.contractNumber}?`)) {
      this.requestService.cancelRequest(request.id).subscribe({
        next: () => this.loadRequests()
      });
    }
  }
}