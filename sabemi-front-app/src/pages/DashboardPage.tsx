import { Card, Col, Row, Table } from "react-bootstrap";
import Page from "../components/layout/Page";
import env from "../config/env";
import { useEffect, useState } from "react";
import type { PaymentWebhookEvent } from "../models/webhook.model";
import paymentEventService, { type PaymentEventFilter } from "../services/paymentEvent.service";
import PaymentWebhookEventTableItem from "../components/common/PaymentWebhookEventTableItem";
import type { Contract } from "../models/contract.model";
import contractService from "../services/contract.service";
import { WebhookEventStatus, WebhookEventStatusLabels } from "../enums/webhook.enum";
import { useSocket } from "../hooks/useSocket";
import { IconCircleCheck, IconCircleX, IconClock, IconRepeat } from "@tabler/icons-react";
import { toast } from "react-toastify";

export default function DashboardPage() {

    const [isLoading, setIsLoading] = useState(false);
    const [filter, setFilter] = useState<PaymentEventFilter>({} as PaymentEventFilter);
    const [events, setEvents] = useState<PaymentWebhookEvent[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [contracts, setContracts] = useState<Contract[]>([]);
    const { connection } = useSocket();

    const eventsCount = events.length;
    const processedCount = events.filter(event => event.status == WebhookEventStatus.Processed).length;
    const pendingCount = events.filter(event => event.status == WebhookEventStatus.Pending).length;
    const processingCount = events.filter(event => event.status == WebhookEventStatus.Processing).length;
    const failedCount = events.filter(event => event.status == WebhookEventStatus.Failed).length;

    useEffect(() => {
        fetchContracts();
    }, [])

    useEffect(() => {
        fetchEvents();
    }, [filter])

    useEffect(() => {

        connection?.on("event-created", () => {
            toast.info("Novo evento de pagamento recebido!");
            fetchEvents();
        });

        connection?.on("event-changed", () => {
            toast.info("Evento de pagamento atualizado!");
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

        setIsLoading(true);
        toast.loading("Carregando eventos de pagamento...", { toastId: "fetch-events" });

        try {
            const result = await paymentEventService.list(filter);
            setEvents(result);
        }
        catch (error) {
            setError("Erro ao carregar eventos de pagamento. Tente novamente mais tarde.");
        }
        finally {
            setIsLoading(false);
            toast.dismiss("fetch-events");
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

            <Row className="mb-3" hidden={Boolean(error)}>
                <Col>
                    <Card>
                        <Card.Body className="bg-secondary text-white">
                            <h6>Eventos</h6>
                            <div className="display-6">{events.length}</div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col>
                    <Card>
                        <Card.Body className="bg-success text-white">
                            <h6>Processados</h6>
                            <div className="display-6">{processedCount}</div>
                            <IconCircleCheck size={56} stroke={1} style={{ position: "absolute", top: 10, right: 10 }} />
                        </Card.Body>
                    </Card>
                </Col>

                <Col>
                    <Card>
                        <Card.Body className="bg-primary text-white">
                            <h6>Processando</h6>
                            <div className="display-6">{processingCount}</div>
                            <IconRepeat size={56} stroke={1} style={{ position: "absolute", top: 10, right: 10 }} />
                        </Card.Body>
                    </Card>
                </Col>

                <Col>
                    <Card>
                        <Card.Body className="bg-warning text-white">
                            <h6>Pendentes</h6>
                            <div className="display-6">{pendingCount}</div>
                            <IconClock size={56} stroke={1} style={{ position: "absolute", top: 10, right: 10 }} />
                        </Card.Body>
                    </Card>
                </Col>

                <Col>
                    <Card>
                        <Card.Body className="bg-danger text-white">
                            <h6>Falhas</h6>
                            <div className="display-6">{failedCount}</div>
                            <IconCircleX size={56} stroke={1} style={{ position: "absolute", top: 10, right: 10 }} />
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            <Card className="mb-3">

                <Card.Header className="py-3">
                    <div className="d-flex justify-content-between gap-3 align-items-center">

                        <select
                            className="form-select"
                            disabled={isLoading}
                            value={filter.contractId}
                            onChange={({ target }) => handleChangeContractFilter(target.value)}
                        >
                            <option value={-1} label={"Todos Contratos"} />
                            {contracts.map(contract => <option key={contract.id} value={contract.id} label={contract.name} />)}
                        </select>

                        <select
                            className="form-select"
                            disabled={isLoading}
                            value={filter.status}
                            onChange={({ target }) => handleChangeStatusFilter(Number(target.value))}
                        >
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
                                <th>Status</th>
                                <th className="text-center">Data/Hora</th>
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