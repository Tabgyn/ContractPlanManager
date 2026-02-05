import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
    PlanChangeRequest,
    CreatePlanChangeRequestDto,
    ProcessPlanChangeRequestDto,
    ApiResponse
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class PlanChangeRequestService {
    private http = inject(HttpClient);
    private baseUrl = `${environment.apiUrl}/planchangerequests`;

    getPendingRequests(): Observable<PlanChangeRequest[]> {
        return this.http
            .get<ApiResponse<PlanChangeRequest[]>>(`${this.baseUrl}/pending`)
            .pipe(map(response => response.data || []));
    }

    getByContractId(contractId: string): Observable<PlanChangeRequest[]> {
        return this.http
            .get<ApiResponse<PlanChangeRequest[]>>(`${this.baseUrl}/contract/${contractId}`)
            .pipe(map(response => response.data || []));
    }

    getById(id: string): Observable<PlanChangeRequest | null> {
        return this.http
            .get<ApiResponse<PlanChangeRequest>>(`${this.baseUrl}/${id}`)
            .pipe(map(response => response.data || null));
    }

    create(dto: CreatePlanChangeRequestDto): Observable<PlanChangeRequest> {
        return this.http
            .post<ApiResponse<PlanChangeRequest>>(this.baseUrl, dto)
            .pipe(map(response => response.data!));
    }

    processRequest(id: string, dto: ProcessPlanChangeRequestDto): Observable<PlanChangeRequest> {
        return this.http
            .post<ApiResponse<PlanChangeRequest>>(`${this.baseUrl}/${id}/process`, dto)
            .pipe(map(response => response.data!));
    }

    cancelRequest(id: string): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.baseUrl}/${id}/cancel`, null)
            .pipe(map(response => response.data || false));
    }

    // For the UI, we'll use getPendingRequests as the default "getAll"
    getAll(): Observable<PlanChangeRequest[]> {
        return this.getPendingRequests();
    }
}