import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { PaymentPlan, CreateContractDto } from '../../../../core/models';
import { ContractService, PaymentPlanService } from '../../../../core/services';

@Component({
  selector: 'app-contract-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './contract-form.html',
  styleUrl: './contract-form.scss'
})
export class ContractForm implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ContractForm>);
  private contractService = inject(ContractService);
  private planService = inject(PaymentPlanService);

  contractForm!: FormGroup;
  paymentPlans = signal<PaymentPlan[]>([]);
  loading = signal(false);

  ngOnInit() {
    this.initForm();
    this.loadPaymentPlans();
  }

  initForm() {
    this.contractForm = this.fb.group({
      contractNumber: ['', [Validators.required, Validators.pattern(/^[A-Z0-9-]+$/)]],
      customerName: ['', [Validators.required, Validators.maxLength(200)]],
      customerEmail: ['', [Validators.required, Validators.email]],
      startDate: [new Date(), Validators.required],
      initialPaymentPlanId: ['', Validators.required]
    });
  }

  loadPaymentPlans() {
    this.planService.getActivePlans().subscribe({
      next: (plans) => this.paymentPlans.set(plans)
    });
  }

  onSubmit() {
    if (this.contractForm.valid) {
      this.loading.set(true);
      const dto: CreateContractDto = this.contractForm.value;

      this.contractService.createContract(dto).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: () => this.loading.set(false)
      });
    }
  }

  onCancel() {
    this.dialogRef.close(false);
  }
}