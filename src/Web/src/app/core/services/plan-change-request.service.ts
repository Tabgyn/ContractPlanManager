import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';
import {
    PlanChangeRequest,
    CreatePlanChangeRequestDto,
    ProcessPlanChangeRequestDto,
    ApiResponse
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class PlanChangeRequestService extends BaseApiService<PlanChangeRequest> {
    protected override endpoint = 'planchangerequests';

    getPendingRequests(): Observable<PlanChangeRequest[]> {
        return this.http
            .get<ApiResponse<PlanChangeRequest[]>>(`${this.apiUrl}/pending`, this.getHttpOptions())
            .pipe(map(response => response.data || []));
    }

    getByContractId(contractId: string): Observable<PlanChangeRequest[]> {
        return this.http
            .get<ApiResponse<PlanChangeRequest[]>>(`${this.apiUrl}/contract/${contractId}`, this.getHttpOptions())
            .pipe(map(response => response.data || []));
    }

    createRequest(dto: CreatePlanChangeRequestDto): Observable<PlanChangeRequest> {
        return this.create(dto);
    }

    processRequest(id: string, dto: ProcessPlanChangeRequestDto): Observable<PlanChangeRequest> {
        return this.http
            .post<ApiResponse<PlanChangeRequest>>(`${this.apiUrl}/${id}/process`, dto, this.getHttpOptions())
            .pipe(map(response => response.data!));
    }

    cancelRequest(id: string): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/cancel`, null, this.getHttpOptions())
            .pipe(map(response => response.data || false));
    }
}