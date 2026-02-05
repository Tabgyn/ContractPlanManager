import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models';

export abstract class BaseApiService<T> {
    protected http = inject(HttpClient);
    protected baseUrl = environment.apiUrl;
    protected abstract endpoint: string;

    protected get apiUrl(): string {
        return `${this.baseUrl}/${this.endpoint}`;
    }

    protected getHttpOptions() {
        return {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
    }

    getAll(): Observable<T[]> {
        return this.http
            .get<ApiResponse<T[]>>(this.apiUrl, this.getHttpOptions())
            .pipe(map(response => response.data || []));
    }

    getById(id: string): Observable<T | null> {
        return this.http
            .get<ApiResponse<T>>(`${this.apiUrl}/${id}`, this.getHttpOptions())
            .pipe(map(response => response.data || null));
    }

    create(item: Partial<T>): Observable<T> {
        return this.http
            .post<ApiResponse<T>>(this.apiUrl, item, this.getHttpOptions())
            .pipe(map(response => response.data!));
    }

    update(id: string, item: Partial<T>): Observable<T> {
        return this.http
            .put<ApiResponse<T>>(`${this.apiUrl}/${id}`, item, this.getHttpOptions())
            .pipe(map(response => response.data!));
    }

    delete(id: string): Observable<boolean> {
        return this.http
            .delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`, this.getHttpOptions())
            .pipe(map(response => response.success));
    }
}