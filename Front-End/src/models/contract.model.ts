import type { ContractStatus } from "../enums/contract.enum";

export interface Contract {
    id: string;
    status: ContractStatus;
    name: string;
    totalAmount: number;
    paidAmount: number;
    pendingAmount: number;
    updatedAt: Date;
    createdAt: Date;

    payments?: ContractPayment[];
}

export interface ContractPayment {
    id: string;
    contractId: string;
    transactionId: string;
    amount: number;
    paidAt: Date;
    createdAt: Date;
    contract?: Contract;
}