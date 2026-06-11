const API_ROUTES = {

    CONTRACTS: {
        BASE: "/contracts",
        BY_ID: (id: string) => `/contracts/${id}`,
    },

    PAYMENT_EVENTS: {
        BASE: "/payment-events",
        REPROCESS: `/payment-events/reprocess`
    }

}

export default API_ROUTES;