import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatGridListModule } from '@angular/material/grid-list';
import { PaymentPlan } from '../../../../core/models';
import { PaymentPlanService } from '../../../../core/services';
import { PaymentPlanCard } from '../payment-plan-card/payment-plan-card';

@Component({
  selector: 'app-payment-plan-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatGridListModule,
    PaymentPlanCard
  ],
  templateUrl: './payment-plan-list.html',
  styleUrl: './payment-plan-list.scss'
})
export class PaymentPlanList implements OnInit {
  private planService = inject(PaymentPlanService);

  plans = signal<PaymentPlan[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.loadPlans();
  }

  loadPlans() {
    this.loading.set(true);
    this.planService.getAll().subscribe({
      next: (plans) => {
        this.plans.set(plans);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  toggleActivation(plan: PaymentPlan) {
    const action = plan.isActive
      ? this.planService.deactivate(plan.id)
      : this.planService.reactivate(plan.id);

    action.subscribe({
      next: () => this.loadPlans()
    });
  }
}