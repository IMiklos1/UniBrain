import Header from "./components/Header"
import { Route, createBrowserRouter, createRoutesFromElements, RouterProvider } from "react-router-dom";
import Calendar from "./pages/Calendar";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Subjects from "./pages/Subjects";

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route path="/" element={<Header />}>
            <Route path="/" element={<Calendar />} />
            <Route path="calendar" element={<Calendar />} />
            <Route path="subjects" element={<Subjects />} />
            <Route path="login" element={<Login />} />
            <Route path="register" element={<Register />} />
        </Route>
    )
)

function App() {

    return (
        <>
            <RouterProvider router={router} />
        </>
    )
}

export default App
