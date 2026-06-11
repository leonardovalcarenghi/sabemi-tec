import type { WebhookEventStatus } from "../enums/webhook.enum";
import api from "../infra/api";
import API_ROUTES from "../infra/api.routes";
import type { PaymentWebhookEvent } from "../models/webhook.model";
import { toQueryParams } from "../utils/formatters";

export interface PaymentEventFilter {
    contractId?: string;
    status?: WebhookEventStatus;
}

const paymentEventService = {

    async list(filter: PaymentEventFilter): Promise<PaymentWebhookEvent[]> {
        const query = toQueryParams(filter);
        return await api.get(`${API_ROUTES.PAYMENT_EVENTS.BASE}${query}`).then(res => res.data);
    },

    async reprocess(transactionId: string): Promise<void> {
        await api.post(API_ROUTES.PAYMENT_EVENTS.REPROCESS, { transactionId });
    }

};

export default paymentEventService;