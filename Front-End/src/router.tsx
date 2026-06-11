import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { APP_ROUTES } from "./config/constants";
import DashboardPage from "./pages/DashboardPage";
import ContractIndexPage from "./pages/ContractIndexPage";
import ContractFormPage from "./pages/ContractFormPage";
import Root from "./components/layout/Root";

export function Router() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<Root />}>

                    {/* Dashboard */}
                    <Route path={APP_ROUTES.DASHBOARD} element={<DashboardPage />} />

                    {/* Contratos */}
                    <Route path={APP_ROUTES.CONTRACTS} element={<ContractIndexPage />} />
                    <Route path={`${APP_ROUTES.CONTRACTS}/novo`} element={<ContractFormPage />} />
                    <Route path={`${APP_ROUTES.CONTRACTS}/:id/editar`} element={<ContractFormPage />} />

                    {/* Outros */}
                    <Route path="/" element={<Navigate to={APP_ROUTES.DASHBOARD} replace />} />
                    <Route path="*" element={<h1>Página não encontrada</h1>} />

                </Route>
            </Routes>
        </BrowserRouter>
    )
}