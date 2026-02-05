import { Component, Inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { PlanChangeRequest, ProcessPlanChangeRequestDto } from '../../../../core/models';
import { PlanChangeRequestService } from '../../../../core/services';

@Component({
  selector: 'app-change-request-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatRadioModule
  ],
  templateUrl: './change-request-dialog.html',
  styleUrl: './change-request-dialog.scss'
})
export class ChangeRequestDialog {
  processForm: FormGroup;
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ChangeRequestDialog>,
    private requestService: PlanChangeRequestService,
    @Inject(MAT_DIALOG_DATA) public request: PlanChangeRequest
  ) {
    this.processForm = this.fb.group({
      decision: ['approve', Validators.required],
      processedBy: ['', Validators.required],
      rejectionReason: ['']
    });

    // Add conditional validation for rejection reason
    this.processForm.get('decision')?.valueChanges.subscribe(decision => {
      const rejectionControl = this.processForm.get('rejectionReason');
      if (decision === 'reject') {
        rejectionControl?.setValidators([Validators.required]);
      } else {
        rejectionControl?.clearValidators();
      }
      rejectionControl?.updateValueAndValidity();
    });
  }

  onSubmit() {
    if (this.processForm.valid) {
      this.loading.set(true);

      const formValue = this.processForm.value;
      const dto: ProcessPlanChangeRequestDto = {
        approved: formValue.decision === 'approve',
        processedBy: formValue.processedBy,
        rejectionReason: formValue.decision === 'reject' ? formValue.rejectionReason : undefined
      };

      this.requestService.processRequest(this.request.id, dto).subscribe({
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