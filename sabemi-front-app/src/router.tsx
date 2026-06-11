import { BrowserRouter, Route, Routes } from "react-router-dom";
import { APP_ROUTES } from "./config/constants";
import HomePage from "./pages/HomePage";
import ContractIndexPage from "./pages/ContractIndexPage";
import ContractFormPage from "./pages/ContractFormPage";
import AboutPage from "./pages/AboutPage";
import Root from "./components/layout/Root";

export function Router() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<Root />}>

                    {/* Início */}
                    <Route path={APP_ROUTES.HOME} element={<HomePage />} />

                    {/* Contratos */}
                    <Route path={APP_ROUTES.CONTRACTS} element={<ContractIndexPage />} />
                    <Route path={`${APP_ROUTES.CONTRACTS}/novo`} element={<ContractFormPage />} />
                    <Route path={`${APP_ROUTES.CONTRACTS}/:id/editar`} element={<ContractFormPage />} />

                    {/* Sobre */}
                    <Route path={APP_ROUTES.ABOUT} element={<AboutPage />} />

                    {/* Outros */}
                    <Route path="*" element={<h1>Página não encontrada</h1>} />

                </Route>
            </Routes>
        </BrowserRouter>
    )
}