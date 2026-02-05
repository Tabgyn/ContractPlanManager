export interface PaymentPlan {
    id: string;
    name: string;
    description: string;
    monthlyPrice: number;
    billingCycle: BillingCycle;
    tier: PlanTier;
    isActive: boolean;
    createdAt: Date;
}

export enum BillingCycle {
    Monthly = 'Monthly',
    Quarterly = 'Quarterly',
    Annually = 'Annually'
}

export enum PlanTier {
    Basic = 'Basic',
    Standard = 'Standard',
    Premium = 'Premium',
    Enterprise = 'Enterprise'
}

export interface CreatePaymentPlanDto {
    name: string;
    description: string;
    monthlyPrice: number;
    billingCycle: BillingCycle;
    tier: PlanTier;
}

export interface UpdatePaymentPlanDto {
    monthlyPrice: number;
    description: string;
}