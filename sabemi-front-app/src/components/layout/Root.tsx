import { Outlet } from "react-router-dom";
import Footer from "./Footer";
import Header from "./Header";
import { SocketProvider } from "../../contexts/SocketContext";

export default function Root() {
    return (
        <>
            <Header />
            <SocketProvider>
                <Outlet />
            </SocketProvider>
            <Footer />
        </>
    )
}