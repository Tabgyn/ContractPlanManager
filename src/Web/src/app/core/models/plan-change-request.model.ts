export interface PlanChangeRequest {
    id: string;
    contractId: string;
    contractNumber: string;
    fromPlanId: string;
    fromPlanName: string;
    toPlanId: string;
    toPlanName: string;
    status: ChangeRequestStatus;
    requestedAt: Date;
    processedAt?: Date;
    requestedBy: string;
    processedBy?: string;
    rejectionReason?: string;
    effectiveDate: Date;
    isUpgrade: boolean;
}

export enum ChangeRequestStatus {
    Pending = 'Pending',
    Approved = 'Approved',
    Rejected = 'Rejected',
    Cancelled = 'Cancelled'
}

export interface CreatePlanChangeRequestDto {
    contractId: string;
    toPlanId: string;
    requestedBy: string;
    effectiveDate: Date;
}

export interface ProcessPlanChangeRequestDto {
    approved: boolean;
    processedBy: string;
    rejectionReason?: string;
}