import { Card, Col, Row, Table } from "react-bootstrap";
import Page from "../components/layout/Page";
import { useEffect, useState } from "react";
import type { Contract } from "../models/contract.model";
import contractService from "../services/contract.service";
import ContractTableItem from "../components/common/ContractTableItem";
import { formatCurrency } from "../utils/formatters";
import { useSocket } from "../hooks/useSocket";

export default function ContractIndexPage() {

    const { connection } = useSocket();
    const [isLoading, setIsLoading] = useState(false);
    const [contracts, setContracts] = useState<Contract[]>([]);
    const [error, setError] = useState<string | null>(null);

    const totalAmount = contracts.reduce((total, contract) => total + contract.totalAmount, 0);
    const paidAmount = contracts.reduce((total, contract) => total + contract.paidAmount, 0);
    const pendingAmount = contracts.reduce((total, contract) => total + contract.pendingAmount, 0);

    useEffect(() => {

        connection?.on("contract-created", () => {
            fetchContracts();
        });

        connection?.on("contract-changed", () => {
            fetchContracts();
        });

        return () => {
            connection?.off("contract-created");
            connection?.off("contract-changed");
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

    useEffect(() => {
        fetchContracts();
    }, [])

    return (
        <Page title="Contratos">

            <Row className="mb-3" hidden={Boolean(error)}>
                <Col lg={3}>
                    <Card>
                        <Card.Body>
                            <h6>Contratos</h6>
                            <div className="display-6">{contracts.length}</div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={3}>
                    <Card>
                        <Card.Body>
                            <h6>Totais</h6>
                            <div className="display-6">{formatCurrency(totalAmount)}</div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={3}>
                    <Card>
                        <Card.Body>
                            <h6>Pagamentos</h6>
                            <div className="display-6">{formatCurrency(paidAmount)}</div>
                        </Card.Body>
                    </Card>
                </Col>

                <Col lg={3}>
                    <Card>
                        <Card.Body>
                            <h6>Pendentes</h6>
                            <div className="display-6">{formatCurrency(pendingAmount)}</div>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            <Card className="mb-3">
                <Card.Header className="py-3">
                    <div className="d-flex justify-content-end align-items-center">
                        <button className="btn btn-primary">
                            Novo Contrato
                        </button>
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

                    <Table striped bordered hover className="mb-0" hidden={Boolean(error)}>
                        <thead>
                            <tr>
                                <th className="text-center">#</th>
                                <th>Nome</th>
                                <th>Valor</th>
                                <th>Pago</th>
                                <th>Pendente</th>
                                <th className="text-center">Status</th>
                                <th className="text-center">Data</th>
                                <th className="text-center">Ações</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                contracts.length === 0 && (
                                    <tr>
                                        <td colSpan={8} className="text-center">Nenhum contrato encontrado.</td>
                                    </tr>
                                )
                            }
                            {contracts.map((contract, index) => <ContractTableItem index={index} key={contract.id} contract={contract} />)}
                        </tbody>
                    </Table>
                </Card.Body>
            </Card>

        </Page>
    )
}