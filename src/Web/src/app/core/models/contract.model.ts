export interface Contract {
    id: string;
    contractNumber: string;
    customerName: string;
    customerEmail: string;
    startDate: Date;
    endDate?: Date;
    status: ContractStatus;
    currentPaymentPlanId: string;
    currentPaymentPlanName: string;
    currentMonthlyPrice: number;
    createdAt: Date;
    updatedAt: Date;
}

export enum ContractStatus {
    Active = 'Active',
    Suspended = 'Suspended',
    Terminated = 'Terminated'
}

export interface CreateContractDto {
    contractNumber: string;
    customerName: string;
    customerEmail: string;
    startDate: Date;
    initialPaymentPlanId: string;
}

export interface UpdateContractDto {
    customerName: string;
    customerEmail: string;
}