import api from "../infra/api";
import API_ROUTES from "../infra/api.routes";
import type { Contract } from "../models/contract.model";

const contractService = {

    async list(): Promise<Contract[]> {
        return await api.get(API_ROUTES.CONTRACTS.BASE).then(res => res.data);
    },

    async getById(id: string): Promise<Contract> {
        return await api.get(API_ROUTES.CONTRACTS.BY_ID(id)).then(res => res.data);
    },

    async create(contract: Contract): Promise<void> {
        return await api.post(API_ROUTES.CONTRACTS.BASE, contract).then(res => res.data);
    },

    async update(id: string, contract: Contract): Promise<void> {
        return await api.put(API_ROUTES.CONTRACTS.BY_ID(id), contract).then(res => res.data);
    }

};

export default contractService;