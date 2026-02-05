import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';
import {
    Contract,
    CreateContractDto,
    UpdateContractDto,
    ApiResponse
} from '../models';

@Injectable({
    providedIn: 'root'
})
export class ContractService extends BaseApiService<Contract> {
    protected override endpoint = 'contracts';

    getActiveContracts(): Observable<Contract[]> {
        return this.http
            .get<ApiResponse<Contract[]>>(`${this.apiUrl}/active`, this.getHttpOptions())
            .pipe(map(response => response.data || []));
    }

    getByContractNumber(contractNumber: string): Observable<Contract | null> {
        return this.http
            .get<ApiResponse<Contract>>(`${this.apiUrl}/by-number/${contractNumber}`, this.getHttpOptions())
            .pipe(map(response => response.data || null));
    }

    createContract(dto: CreateContractDto): Observable<Contract> {
        return this.create(dto);
    }

    updateContract(id: string, dto: UpdateContractDto): Observable<Contract> {
        return this.update(id, dto);
    }

    suspend(id: string): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/suspend`, null, this.getHttpOptions())
            .pipe(map(response => response.data || false));
    }

    reactivate(id: string): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/reactivate`, null, this.getHttpOptions())
            .pipe(map(response => response.data || false));
    }

    terminate(id: string, endDate: Date): Observable<boolean> {
        return this.http
            .post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/terminate`, endDate, this.getHttpOptions())
            .pipe(map(response => response.data || false));
    }
}