import { Component, Inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { PaymentPlan, CreatePlanChangeRequestDto } from '../../../../core/models';
import { PaymentPlanService, PlanChangeRequestService } from '../../../../core/services';

@Component({
  selector: 'app-create-change-request-dialog',
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
  templateUrl: './create-change-request-dialog.html',
  styleUrl: './create-change-request-dialog.scss'
})
export class CreateChangeRequestDialog implements OnInit {
  requestForm: FormGroup;
  paymentPlans = signal<PaymentPlan[]>([]);
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CreateChangeRequestDialog>,
    private planService: PaymentPlanService,
    private requestService: PlanChangeRequestService,
    @Inject(MAT_DIALOG_DATA) public data: { contractId: string }
  ) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    this.requestForm = this.fb.group({
      toPlanId: ['', Validators.required],
      requestedBy: ['', [Validators.required, Validators.email]],
      effectiveDate: [tomorrow, Validators.required]
    });
  }

  ngOnInit() {
    this.loadPaymentPlans();
  }

  loadPaymentPlans() {
    this.planService.getActivePlans().subscribe({
      next: (plans) => this.paymentPlans.set(plans)
    });
  }

  onSubmit() {
    if (this.requestForm.valid) {
      this.loading.set(true);

      const dto: CreatePlanChangeRequestDto = {
        contractId: this.data.contractId,
        toPlanId: this.requestForm.value.toPlanId,
        requestedBy: this.requestForm.value.requestedBy,
        effectiveDate: this.requestForm.value.effectiveDate
      };

      this.requestService.create(dto).subscribe({
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