import type { WebhookEventStatus } from "../enums/webhook.enum";
import type { Contract } from "./contract.model";

export interface PaymentWebhookEvent {
    id: string;
    contractId: string;
    transactionId: string;
    payload: string;
    status: WebhookEventStatus;
    retryCount: number;
    errorMessage: string | null;
    processedAt: Date;
    createdAt: Date;

    contract?: Contract;
}