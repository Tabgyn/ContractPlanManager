import { Routes } from '@angular/router';
import { PaymentPlanList } from './components/payment-plan-list/payment-plan-list';

export const PAYMENT_PLANS_ROUTES: Routes = [
    {
        path: '',
        component: PaymentPlanList
    }
];