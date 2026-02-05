import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Contract } from '../../../../core/models';
import { ContractService } from '../../../../core/services';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { ContractForm } from '../contract-form/contract-form';

@Component({
  selector: 'app-contract-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './contract-list.html',
  styleUrl: './contract-list.scss',
})
export class ContractList implements OnInit {
  private contractService = inject(ContractService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  contracts = signal<Contract[]>([]);
  loading = signal(true);
  displayedColumns = ['contractNumber', 'customerName', 'customerEmail', 'status', 'currentPlan', 'monthlyPrice', 'actions'];

  ngOnInit() {
    this.loadContracts();
  }

  loadContracts() {
    this.loading.set(true);
    this.contractService.getAll().subscribe({
      next: (contracts) => {
        this.contracts.set(contracts);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  viewDetails(contract: Contract) {
    this.router.navigate(['/contracts', contract.id]);
  }

  openCreateDialog() {
    const dialogRef = this.dialog.open(ContractForm, {
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadContracts();
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Active':
        return 'primary';
      case 'Suspended':
        return 'warn';
      case 'Terminated':
        return 'accent';
      default:
        return '';
    }
  }

  suspend(contract: Contract, event: Event) {
    event.stopPropagation();
    if (confirm(`Suspend contract ${contract.contractNumber}?`)) {
      this.contractService.suspend(contract.id).subscribe({
        next: () => this.loadContracts()
      });
    }
  }

  reactivate(contract: Contract, event: Event) {
    event.stopPropagation();
    if (confirm(`Reactivate contract ${contract.contractNumber}?`)) {
      this.contractService.reactivate(contract.id).subscribe({
        next: () => this.loadContracts()
      });
    }
  }
}
