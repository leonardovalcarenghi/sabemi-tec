import { useState } from "react";
import type { Contract } from "../../models/contract.model";
import { formatCurrency, formatDate } from "../../utils/formatters";
import { ContractStatusLabels } from "../../enums/contract.enum";

interface ContractTableItemProps {
    contract: Contract;
    index: number;
}

export default function ContractTableItem({ contract, index }: ContractTableItemProps) {

    const [showDetails, setShowDetails] = useState(false);

    return (
        <>
            <tr>
                <td className="text-center">
                    {index + 1}
                </td>
                <td>
                    {contract.name}
                </td>
                <td>
                    {formatCurrency(contract.totalAmount)}
                </td>
                <td>
                    {formatCurrency(contract.paidAmount)}
                </td>
                <td>
                    {formatCurrency(contract.pendingAmount)}
                </td>
                <td className="text-center">
                    {ContractStatusLabels[contract.status]}
                </td>
                <td className="text-center">
                    {formatDate(contract.createdAt)}
                </td>
                <td className="text-center">

                    <button className={`btn btn-sm ${showDetails ? "btn-secondary" : "btn-outline-secondary"}`} onClick={() => setShowDetails(!showDetails)}>
                        {showDetails ? "Ocultar Pagamentos" : "Mostrar Pagamentos"}
                    </button>

                </td>
            </tr>
            <tr hidden={!showDetails}>
                <td colSpan={8} className="p-0">
                    <div>

                        <table className="table mb-0">
                            <thead hidden={contract.payments?.length === 0}>
                                <tr>
                                    <th className="text-center">
                                        Data
                                    </th>
                                    <th className="text-center">
                                        Valor
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                    contract.payments?.length === 0 && (
                                        <tr>
                                            <td colSpan={2} className="text-center">Nenhum pagamento registrado.</td>
                                        </tr>
                                    )
                                }
                                {
                                    contract.payments?.map(payment => (
                                        <tr key={payment.id}>
                                            <td className="text-center">{formatDate(payment.paidAt)}</td>
                                            <td className="text-center">{formatCurrency(payment.amount)}</td>
                                        </tr>
                                    ))
                                }
                            </tbody>
                        </table>

                    </div>
                </td>
            </tr>
        </>
    )
}