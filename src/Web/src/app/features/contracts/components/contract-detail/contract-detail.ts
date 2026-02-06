import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { Contract, PlanChangeRequest } from '../../../../core/models';
import { ContractService, PlanChangeRequestService } from '../../../../core/services';
import { CreateChangeRequestDialog } from '../create-change-request-dialog/create-change-request-dialog';

@Component({
  selector: 'app-contract-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatListModule
  ],
  templateUrl: './contract-detail.html',
  styleUrl: './contract-detail.scss'
})
export class ContractDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private contractService = inject(ContractService);
  private changeRequestService = inject(PlanChangeRequestService);
  private dialog = inject(MatDialog);

  contract = signal<Contract | null>(null);
  changeRequests = signal<PlanChangeRequest[]>([]);
  loading = signal(true);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadContract(id);
      this.loadChangeRequests(id);
    }
  }

  loadContract(id: string) {
    this.loading.set(true);
    this.contractService.getById(id).subscribe({
      next: (contract) => {
        this.contract.set(contract);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.router.navigate(['/contracts']);
      }
    });
  }

  loadChangeRequests(contractId: string) {
    this.changeRequestService.getByContractId(contractId).subscribe({
      next: (requests) => this.changeRequests.set(requests)
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Active': return 'primary';
      case 'Suspended': return 'warn';
      case 'Terminated': return 'accent';
      default: return '';
    }
  }

  getRequestStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'accent';
      case 'Approved': return 'primary';
      case 'Rejected': return 'warn';
      case 'Cancelled': return '';
      default: return '';
    }
  }

  goBack() {
    this.router.navigate(['/contracts']);
  }

  suspend() {
    const contract = this.contract();
    if (contract && confirm(`Suspend contract ${contract.contractNumber}?`)) {
      this.contractService.suspend(contract.id).subscribe({
        next: () => this.loadContract(contract.id)
      });
    }
  }

  reactivate() {
    const contract = this.contract();
    if (contract && confirm(`Reactivate contract ${contract.contractNumber}?`)) {
      this.contractService.reactivate(contract.id).subscribe({
        next: () => this.loadContract(contract.id)
      });
    }
  }

  terminate() {
    const contract = this.contract();
    if (contract && confirm(`Terminate contract ${contract.contractNumber}? This cannot be undone.`)) {
      const endDate = new Date();
      this.contractService.terminate(contract.id, endDate).subscribe({
        next: () => this.loadContract(contract.id)
      });
    }
  }

  requestPlanChange() {
    const contract = this.contract();
    if (!contract) return;

    const dialogRef = this.dialog.open(CreateChangeRequestDialog, {
      width: '500px',
      data: { contractId: contract.id }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadChangeRequests(contract.id);
      }
    });
  }
}