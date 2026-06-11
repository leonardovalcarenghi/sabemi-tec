import { useParams } from "react-router-dom";
import Page from "../components/layout/Page";

export default function ContractFormPage() {

    const { id } = useParams();
    const isEditing = Boolean(id);
    const pageTitle = isEditing ? "Editar Contrato" : "Novo Contrato";

    return (
        <Page title={pageTitle}>
            <h1>Contract Form Page</h1>
        </Page>
    )
}