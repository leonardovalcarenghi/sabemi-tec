import { useEffect, useState } from "react";
import { WebhookEventStatus, WebhookEventStatusColors, WebhookEventStatusLabels } from "../../enums/webhook.enum";
import type { PaymentWebhookEvent } from "../../models/webhook.model";
import { formatDate, formatDateWithTime } from "../../utils/formatters";
import { useSocket } from "../../hooks/useSocket";
import { IconCircleCheck, IconCircleX, IconClock, IconRepeat } from "@tabler/icons-react";
import paymentEventService from "../../services/paymentEvent.service";
import { toast } from "react-toastify";

const WebhookEventStatusIcons: Record<WebhookEventStatus, React.ReactNode> = {
    [WebhookEventStatus.None]: null,
    [WebhookEventStatus.Pending]: <IconClock className="bi me-2" />,
    [WebhookEventStatus.Processed]: <IconCircleCheck className="bi me-2" />,
    [WebhookEventStatus.Processing]: <IconRepeat className="bi me-2" />,
    [WebhookEventStatus.Failed]: <IconCircleX className="bi me-2" />,
}

interface PaymentWebhookEventTableItemProps {
    index: number;
    event: PaymentWebhookEvent;
}

export default function PaymentWebhookEventTableItem({ index, event }: PaymentWebhookEventTableItemProps) {

    const { connection } = useSocket();
    const signalEventName = `event-changed-#${event.transactionId}`;

    const [statusHighlight, setStatusHighlight] = useState<boolean>(false);
    const [showDetails, setShowDetails] = useState(false);

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

    const handleReprocess = async () => {

        const confirmed = window.confirm("Tem certeza que deseja reprocessar este evento?");
        if (!confirmed) return;

        try {
            await paymentEventService.reprocess(event.transactionId);
            toast.success("Evento enviado para reprocessamento!");
        }
        catch (error) {
            toast.error("Erro ao enviar evento para reprocessamento.");
        }
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
                <td className={`${WebhookEventStatusColors[event.status]} ${statusHighlight ? "border border-3 border-warning" : ""}`}>

                    <div className="d-flex align-items-center justify-content-between">
                        <span>{WebhookEventStatusLabels[event.status]}</span>
                        {WebhookEventStatusIcons[event.status]}

                    </div>

                </td>
                <td className="text-center">
                    {formatDateWithTime(event.createdAt)}
                </td>

                <td>
                    <div className="d-flex gap-2 justify-content-center">
                        {
                            event.status === WebhookEventStatus.Failed &&
                            <button className="btn btn-outline-primary btn-sm" onClick={handleReprocess}>
                                Reprocessar
                            </button>
                        }

                        <button className={`btn btn-sm ${showDetails ? "btn-secondary" : "btn-outline-secondary"}`} onClick={() => setShowDetails(!showDetails)}>
                            {showDetails ? "Ocultar Detalhes" : "Mostrar Detalhes"}
                        </button>
                    </div>
                </td>
            </tr>
            <tr hidden={!showDetails}>
                <td colSpan={6} className="p-5">
                    <div className="d-flex flex-column gap-3">

                        <div className="d-grid">
                            <label>Data do Processamento:</label>
                            <span>{event.processedAt ? formatDateWithTime(event.processedAt) : "-"}</span>
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