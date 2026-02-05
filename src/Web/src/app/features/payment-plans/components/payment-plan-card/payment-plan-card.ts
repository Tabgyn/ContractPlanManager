import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { PaymentPlan } from '../../../../core/models';

@Component({
  selector: 'app-payment-plan-card',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './payment-plan-card.html',
  styleUrl: './payment-plan-card.scss'
})
export class PaymentPlanCard {
  @Input({ required: true }) plan!: PaymentPlan;
  @Output() toggleActivation = new EventEmitter<void>();

  getTierColor(tier: string): string {
    switch (tier) {
      case 'Basic':
        return 'accent';
      case 'Standard':
        return 'primary';
      case 'Premium':
        return 'warn';
      case 'Enterprise':
        return '';
      default:
        return '';
    }
  }

  onToggleActivation() {
    this.toggleActivation.emit();
  }
}