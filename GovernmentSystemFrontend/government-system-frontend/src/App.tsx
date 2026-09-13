import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { ProtectedRoute, PublicRoute } from "./components/auth/RouteGuards";

// Placeholder components for now
import Login from "./pages/Login";
import Home from "./pages/Home";

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public-only routes (e.g., Login) */}
        <Route element={<PublicRoute />}>
          <Route path="/login" element={<Login />} />
        </Route>

        {/* Protected Feature routes (Dashboard, CRUDs, etc.) */}
        <Route element={<ProtectedRoute />}>
          <Route path="/home" element={<Home />} />
          {/* Future CRUD routes will go here */}
        </Route>

        {/* Catch-all redirect */}
        <Route path="*" element={<Navigate to="/home" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
