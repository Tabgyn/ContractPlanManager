import { Routes } from '@angular/router';
import { ContractList } from './components/contract-list/contract-list';
import { ContractDetail } from './components/contract-detail/contract-detail';

export const CONTRACTS_ROUTES: Routes = [
    {
        path: '',
        component: ContractList
    },
    {
        path: ':id',
        component: ContractDetail
    }
];