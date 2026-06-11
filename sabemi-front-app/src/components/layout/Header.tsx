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
                <Navbar.Brand href={APP_ROUTES.DASHBOARD}>{env.APP_NAME}</Navbar.Brand>
                <Nav className="ms-auto">

                    <NavLink to={APP_ROUTES.DASHBOARD} className="nav-link">
                        <span>Dashboard</span>
                    </NavLink>

                    <NavLink to={APP_ROUTES.CONTRACTS} className="nav-link">
                        <span>Contratos</span>
                    </NavLink>
           
                </Nav>
            </Container>
        </Navbar>
    )
}