import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { PublicRoute } from "./components/PublicRoute";
import { LoginPage } from "./pages/LoginPage";
import { CitizensPage } from "./pages/CitizensPage";

// --- Temporary Local Components (Will be replaced as we build each page) ---

const VotersPagePlaceholder = () => {
  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-slate-900">
        Voters Management Placeholder
      </h1>
    </div>
  );
};

const CandidatesPagePlaceholder = () => {
  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-slate-900">
        Candidates Management Placeholder
      </h1>
    </div>
  );
};

// --- Query Client Configuration ---

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: false,
    },
  },
});

// --- App Root Component ---

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          {/* Public Routes */}
          <Route element={<PublicRoute />}>
            <Route path="/login" element={<LoginPage />} />
          </Route>

          {/* Protected Routes */}
          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<Navigate to="/citizens" replace />} />
            <Route path="/citizens" element={<CitizensPage />} />
            <Route path="/voters" element={<VotersPagePlaceholder />} />
            <Route path="/candidates" element={<CandidatesPagePlaceholder />} />
          </Route>

          {/* Fallback 404 */}
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>

      <Toaster position="top-right" />
    </QueryClientProvider>
  );
}

export default App;
