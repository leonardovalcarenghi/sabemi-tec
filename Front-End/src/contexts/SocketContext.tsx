import { HubConnectionBuilder, HubConnectionState, LogLevel, type HubConnection } from "@microsoft/signalr";
import { createContext, useCallback, useEffect, useRef, useState } from "react";
import env from "../config/env";

// ─── Tipos do contexto ────────────────────────────────────────────────────────

export interface SocketContextValue {
    connection: HubConnection | null;
    isConnected: boolean;
}

// ─── Contexto ─────────────────────────────────────────────────────────────────

export const SocketContext = createContext<SocketContextValue | null>(null);

// ─── Provider ─────────────────────────────────────────────────────────────────

export function SocketProvider({ children }: { children: React.ReactNode }) {

    const hubConnectionRef = useRef<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    const startConnection = useCallback(
        async () => {

            if (hubConnectionRef.current?.state === HubConnectionState.Connected)
                return;

            console.log('Iniciando conexão com SignalR...');
            console.log('URL do Hub:', env.NOTIFICATIONS_HUB_URL);

            const connection = new HubConnectionBuilder()
                .withUrl(env.NOTIFICATIONS_HUB_URL)
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Warning)
                .build();
       
            connection.onreconnected(() => setIsConnected(true));
            connection.onreconnecting(() => setIsConnected(false));
            connection.onclose(() => setIsConnected(false));

            try {
                await connection.start();
                hubConnectionRef.current = connection;
                setIsConnected(true);
            } catch {
                setIsConnected(false);
            }
        }, []
    );

    const stopConnection = useCallback(
        async () => {
            if (hubConnectionRef.current) {
                await hubConnectionRef.current.stop()
                hubConnectionRef.current = null
            }
        }, []
    )

    useEffect(() => {
        startConnection();

        return () => {
            stopConnection();
        };

    }, [startConnection]);


    return (
        <SocketContext.Provider value={{ connection: hubConnectionRef.current, isConnected }}>
            {children}
        </SocketContext.Provider>
    );
}