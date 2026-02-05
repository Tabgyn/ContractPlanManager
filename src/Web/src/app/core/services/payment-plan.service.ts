import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';
import {
    PaymentPlan,
    CreatePaymentPlanDto,
    UpdatePaymentPlanDto,
    ApiResponse
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class PaymentPlanService extends BaseApiService<PaymentPlan> {
    protected override endpoint = 'paymentplans';

    getActivePlans(): Observable<PaymentPlan[]> {
        return this.http
            .get<ApiResponse<PaymentPlan[]>>(`${this.apiUrl}/active`, this.getHttpOptions())
            .pipe(map(response => response.data || []));
    }

    createPlan(dto: CreatePaymentPlanDto): Observable<PaymentPlan> {
        return this.create(dto);
    }

    updatePlan(id: string, dto: UpdatePaymentPlanDto): Observable<PaymentPlan> {
        return this.update(id, dto);
    }

    deactivate(id: string): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/deactivate`, null, this.getHttpOptions())
            .pipe(map(response => response.data || false));
    }

    reactivate(id: string): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/reactivate`, null, this.getHttpOptions())
            .pipe(map(response => response.data || false));
    }
}