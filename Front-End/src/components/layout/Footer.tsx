import { Col, Container } from "react-bootstrap";

interface FooterProps {

}

export default function Footer({ }: FooterProps) {
    return (
        <Container>
            <footer className="d-flex flex-wrap justify-content-between align-items-center py-3 my-4 border-top">
                <Col md="4" className="d-flex align-items-center">
                    <span className="mb-3 mb-md-0 text-body-secondary">
                        2026 - Desenvolvido por <a target="_blank" href="https://www.linkedin.com/in/leonardo-valcarenghi/">Leonardo Valcarenghi</a>
                    </span>
                </Col>
            </footer>
        </Container>

    )
}