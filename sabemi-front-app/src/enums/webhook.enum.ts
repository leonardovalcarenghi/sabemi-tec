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