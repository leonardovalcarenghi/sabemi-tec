export enum WebhookEventStatus {
    None = 0,
    Pending = 1,
    Processed = 2,
    Processing = 3,
    Failed = 4
}


export const WebhookEventStatusLabels: Record<WebhookEventStatus, string> = {
    [WebhookEventStatus.None]: "Desconhecido",
    [WebhookEventStatus.Pending]: "Pendente",
    [WebhookEventStatus.Processed]: "Processado",
    [WebhookEventStatus.Processing]: "Processando",
    [WebhookEventStatus.Failed]: "Falhou",
}

export const WebhookEventStatusColors: Record<WebhookEventStatus, string> = {
    [WebhookEventStatus.None]: "",
    [WebhookEventStatus.Pending]: "",
    [WebhookEventStatus.Processed]: "",
    [WebhookEventStatus.Processing]: "bg-primary text-white",
    [WebhookEventStatus.Failed]: "bg-danger text-white",
}