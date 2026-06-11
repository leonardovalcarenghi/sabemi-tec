import { useEffect, useState } from "react";
import { WebhookEventStatusLabels } from "../../enums/webhook.enum";
import type { PaymentWebhookEvent } from "../../models/webhook.model";
import { formatDate } from "../../utils/formatters";
import { useSocket } from "../../hooks/useSocket";

interface PaymentWebhookEventTableItemProps {
    index: number;
    event: PaymentWebhookEvent;
}

export default function PaymentWebhookEventTableItem({ index, event }: PaymentWebhookEventTableItemProps) {

    const { connection } = useSocket();

    const signalEventName = `event-changed-#${event.transactionId}`;

    const [statusHighlight, setStatusHighlight] = useState<boolean>(false);
    const [showDetails, setShowDetails] = useState(false);

    // to do: aqui dentro preciso fazer ele receber via sinalr a atualização do transaction. 

    useEffect(() => {

        connection?.on(signalEventName, () => {
            showStatusChanged();
        });

        return () => {
            connection?.off(signalEventName);
        }

    }, [connection])


    const showStatusChanged = () => {
        setStatusHighlight(true);
        setInterval(() => { setStatusHighlight(false) }, 8000);
    }


    return (
        <>
            <tr>
                <td className="text-center">
                    {index + 1}
                </td>
                <td>
                    {event.contract?.name || "-"}
                </td>
                <td>
                    {event.transactionId}
                </td>
                <td className={"text-center " + (statusHighlight ? "bg-warning" : "")}>
                    {WebhookEventStatusLabels[event.status]}
                </td>
                <td className="text-center">
                    {formatDate(event.createdAt)}
                </td>

                <td className="text-center">

                    <button className={`btn btn-sm ${showDetails ? "btn-secondary" : "btn-outline-secondary"}`} onClick={() => setShowDetails(!showDetails)}>
                        {showDetails ? "Ocultar Detalhes" : "Mostrar Detalhes"}
                    </button>

                </td>
            </tr>
            <tr hidden={!showDetails}>
                <td colSpan={6} className="p-5">
                    <div className="d-flex flex-column gap-3">

                        <div className="d-grid">
                            <label>Data do Processamento:</label>
                            <span>{formatDate(event.processedAt)}</span>
                        </div>

                        <div className="d-grid">
                            <label>Número de Tentativas:</label>
                            <span>{event.retryCount}</span>
                        </div>

                        <div className="d-grid">
                            <label>Mensagem de Erro:</label>
                            <span>{event.errorMessage || "-"}</span>
                        </div>

                        <div className="d-grid">
                            <label>Payload:</label>
                            <code>{event.payload || "-"}</code>
                        </div>


                    </div>
                </td>
            </tr>
        </>
    )
}