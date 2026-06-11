const API_ROUTES = {

    CONTRACTS: {
        BASE: "/contracts",
        BY_ID: (id: string) => `/contracts/${id}`,
    },

    WEBHOOKS: {
        BASE: "/webhooks",
        FIND_PAYMENT_EVENTS: "/webhooks/payments/list"
    }

}

export default API_ROUTES;