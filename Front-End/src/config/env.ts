const env = {
    APP_NAME: import.meta.env.VITE_APP_NAME || "Sabemi Front App",
    API_BASE_URL: import.meta.env.VITE_API_BASE_URL || "https://localhost:7004",
    NOTIFICATIONS_HUB_URL: import.meta.env.VITE_NOTIFICATIONS_HUB_URL || "https://localhost:7004/hubs/notifications",
};

export default env;