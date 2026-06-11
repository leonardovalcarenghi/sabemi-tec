import { Card, Row, Table } from "react-bootstrap";
import Page from "../components/layout/Page";
import env from "../config/env";
import { useEffect, useState } from "react";
import type { PaymentWebhookEvent } from "../models/webhook.model";
import webhookService, { type PaymentEventFilter } from "../services/webhook.service";
import PaymentWebhookEventTableItem from "../components/common/PaymentWebhookEventTableItem";
import type { Contract } from "../models/contract.model";
import contractService from "../services/contract.service";
import { WebhookEventStatus, WebhookEventStatusLabels } from "../enums/webhook.enum";
import { useSocket } from "../hooks/useSocket";

export default function HomePage() {

    const [isLoading, setIsLoading] = useState(false);
    const [filter, setFilter] = useState<PaymentEventFilter>({} as PaymentEventFilter);
    const [events, setEvents] = useState<PaymentWebhookEvent[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [contracts, setContracts] = useState<Contract[]>([]);
    const { connection } = useSocket();

    useEffect(() => {
        fetchContracts();
    }, [])

    useEffect(() => {
        fetchEvents();
    }, [filter])

    useEffect(() => {

        connection?.on("event-created", () => {
            fetchEvents();
        });

        connection?.on("event-changed", () => {
            fetchEvents();
        });

        return () => {
            connection?.off("event-created");
            connection?.off("event-changed");
        }

    }, [connection]);

    const fetchContracts = async () => {
        try {
            setIsLoading(true);
            const result = await contractService.list();
            setContracts(result);
        }
        catch (error) {
            setError("Erro ao carregar contratos. Tente novamente mais tarde.");
        }
        finally {
            setIsLoading(false);
        }
    }

    const fetchEvents = async () => {
        try {
            setIsLoading(true);
            const result = await webhookService.listPaymentEvents(filter);
            setEvents(result);
        }
        catch (error) {
            setError("Erro ao carregar eventos de pagamento. Tente novamente mais tarde.");
        }
        finally {
            setIsLoading(false);
        }
    }



    // -----------------------

    const handleChangeContractFilter = (value: string) => {
        setFilter(prev => ({
            ...prev,
            contractId: value == "-1" ? undefined : value
        }))
    }

    const handleChangeStatusFilter = (value: number) => {
        setFilter(prev => ({
            ...prev,
            status: value == -1 ? undefined : Number(value) as WebhookEventStatus
        }))
    }


    return (
        <Page title="Início">

            <Card className="mb-3">
                <Card.Body>
                    <Card.Title>Bem-vindo ao {env.APP_NAME}!</Card.Title>
                    <Card.Text>
                        Este é um aplicativo de exemplo para gerenciamento de contratos, desenvolvido com React e TypeScript.
                    </Card.Text>
                </Card.Body>
            </Card>

            <Card className="mb-3">

                <Card.Header className="py-3">
                    <div className="d-flex justify-content-between gap-3 align-items-center">

                        <select className="form-select" value={filter.contractId} onChange={({ target }) => handleChangeContractFilter(target.value)}>
                            <option value={-1} label={"Todos Contratos"} />
                            {contracts.map(contract => <option key={contract.id} value={contract.id} label={contract.name} />)}
                        </select>

                        <select className="form-select" value={filter.status} onChange={({ target }) => handleChangeStatusFilter(Number(target.value))}>
                            <option value={-1} label={"Todos Status"} />
                            <option value={WebhookEventStatus.Pending} label={WebhookEventStatusLabels[WebhookEventStatus.Pending]} />
                            <option value={WebhookEventStatus.Processing} label={WebhookEventStatusLabels[WebhookEventStatus.Processing]} />
                            <option value={WebhookEventStatus.Processed} label={WebhookEventStatusLabels[WebhookEventStatus.Processed]} />
                            <option value={WebhookEventStatus.Failed} label={WebhookEventStatusLabels[WebhookEventStatus.Failed]} />
                        </select>

                    </div>
                </Card.Header>

                <Card.Body>

                    {
                        error && (
                            <div className="alert alert-danger mb-0" role="alert">
                                {error}
                            </div>
                        )
                    }

                    <Table bordered hover className="mb-0" hidden={Boolean(error)}>
                        <thead>
                            <tr>
                                <th className="text-center">#</th>
                                <th>Contrato</th>
                                <th>Transação</th>
                                <th className="text-center">Status</th>
                                <th className="text-center">Data</th>
                                <th className="text-center">Ações</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                events.length === 0 && (
                                    <tr>
                                        <td colSpan={6} className="text-center">Nenhum evento encontrado.</td>
                                    </tr>
                                )
                            }
                            {events.map((event, index) => <PaymentWebhookEventTableItem index={index} key={event.id} event={event} />)}
                        </tbody>
                    </Table>
                </Card.Body>
            </Card>


        </Page>
    )
}