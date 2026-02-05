import { Routes } from '@angular/router';
import { ChangeRequestList } from './components/change-request-list/change-request-list';

export const CHANGE_REQUESTS_ROUTES: Routes = [
    {
        path: '',
        component: ChangeRequestList
    }
];