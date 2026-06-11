export enum ContractStatus {
    None = 0,
    Pending = 1,
    InProgress = 2,
    Completed = 3
}

export const ContractStatusLabels: Record<ContractStatus, string> = {
    [ContractStatus.None]: "Desconhecido",
    [ContractStatus.Pending]: "Pendente",
    [ContractStatus.InProgress]: "Em Andamento",
    [ContractStatus.Completed]: "Concluído",
}