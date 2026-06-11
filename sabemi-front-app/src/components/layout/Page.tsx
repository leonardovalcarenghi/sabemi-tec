import { Col, Container, Row } from "react-bootstrap";

interface PageProps {
    title: string;
    children: React.ReactNode;
}

export default function Page({ title, children }: PageProps) {
    return (
        <Container className="py-3">

            <Row className="mb-5 py-3 border-bottom">
                <Col>
                    <h3>{title}</h3>
                </Col>
            </Row>

            {children}
        </Container>
    )
}