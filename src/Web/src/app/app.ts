import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ContractService, PaymentPlanService } from './core/services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('Web');

  private contractService = inject(ContractService);
  private planService = inject(PaymentPlanService);

  ngOnInit() {
    // Test API connection
    this.contractService.getAll().subscribe({
      next: (contracts) => console.log('Contracts:', contracts),
      error: (error) => console.error('Error loading contracts:', error)
    });

    this.planService.getAll().subscribe({
      next: (plans) => console.log('Payment Plans:', plans),
      error: (error) => console.error('Error loading plans:', error)
    });
  }
}
