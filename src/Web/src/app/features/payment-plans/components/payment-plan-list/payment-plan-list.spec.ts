import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentPlanList } from './payment-plan-list';

describe('PaymentPlanList', () => {
  let component: PaymentPlanList;
  let fixture: ComponentFixture<PaymentPlanList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentPlanList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentPlanList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
