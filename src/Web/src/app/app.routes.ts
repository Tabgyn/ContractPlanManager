import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/contracts',
        pathMatch: 'full'
    },
    {
        path: 'contracts',
        loadChildren: () => import('./features/contracts/contracts.routes').then(m => m.CONTRACTS_ROUTES)
    },
    {
        path: 'payment-plans',
        loadChildren: () => import('./features/payment-plans/payment-plans.routes').then(m => m.PAYMENT_PLANS_ROUTES)
    },
    {
        path: 'change-requests',
        loadChildren: () => import('./features/plan-change-requests/plan-change-requests.routes').then(m => m.CHANGE_REQUESTS_ROUTES)
    },
    {
        path: '**',
        redirectTo: '/contracts'
    }
];