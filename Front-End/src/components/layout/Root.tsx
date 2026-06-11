import { Outlet } from "react-router-dom";
import Footer from "./Footer";
import Header from "./Header";
import { SocketProvider } from "../../contexts/SocketContext";
import { ToastContainer } from "react-toastify";


export default function Root() {
    return (
        <>
            <ToastContainer position="top-center" autoClose={5000} toastStyle={{ width: 400 }} newestOnTop pauseOnFocusLoss closeOnClick draggable />
            <Header />
            <SocketProvider>
                <Outlet />
            </SocketProvider>
            <Footer />
        </>
    )
}