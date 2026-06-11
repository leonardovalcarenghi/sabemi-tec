import { Container, Nav, Navbar } from "react-bootstrap";
import { APP_ROUTES } from "../../config/constants";
import { NavLink } from "react-router-dom";
import env from "../../config/env";

interface HeaderProps {

}

export default function Header({ }: HeaderProps) {
    return (
        <Navbar className="border-bottom">
            <Container>
                <Navbar.Brand href={APP_ROUTES.HOME}>{env.APP_NAME}</Navbar.Brand>
                <Nav className="ms-auto">

                    <NavLink to={APP_ROUTES.HOME} className="nav-link">
                        <span>Início</span>
                    </NavLink>

                    <NavLink to={APP_ROUTES.CONTRACTS} className="nav-link">
                        <span>Contratos</span>
                    </NavLink>

                    <NavLink to={APP_ROUTES.ABOUT} className="nav-link">
                        <span>Sobre</span>
                    </NavLink>

                </Nav>
            </Container>
        </Navbar>
    )
}