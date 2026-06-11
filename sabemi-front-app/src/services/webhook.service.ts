import type { WebhookEventStatus } from "../enums/webhook.enum";
import api from "../infra/api";
import API_ROUTES from "../infra/api.routes";
import type { PaymentWebhookEvent } from "../models/webhook.model";
import { toQueryParams } from "../utils/formatters";

export interface PaymentEventFilter {
    contractId?: string;
    status?: WebhookEventStatus;
}

const webhookService = {

    async listPaymentEvents(filter: PaymentEventFilter): Promise<PaymentWebhookEvent[]> {
        const query = toQueryParams(filter);
        return await api.get(`${API_ROUTES.WEBHOOKS.FIND_PAYMENT_EVENTS}${query}`).then(res => res.data);
    },

};

export default webhookService;