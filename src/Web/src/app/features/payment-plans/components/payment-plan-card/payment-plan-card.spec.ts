import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentPlanCard } from './payment-plan-card';

describe('PaymentPlanCard', () => {
  let component: PaymentPlanCard;
  let fixture: ComponentFixture<PaymentPlanCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentPlanCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentPlanCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
