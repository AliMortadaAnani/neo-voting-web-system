import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./hooks/useAuth";
import { GuestRoute } from "./components/GuestRoute";
import { RoleRoute } from "./components/RoleRoute";
import { AdminPage } from "./pages/AdminPage";
import { VoterPage } from "./pages/VoterPage";
import { CandidatePage } from "./pages/CandidatePage";
import { RegisterVoterPage } from "./pages/RegisterVoterPage";
import { RegisterCandidatePage } from "./pages/RegisterCandidatePage";
// --- Local Placeholder Components ---

import { LoginPage } from "./pages/LoginPage";

const AboutPagePlaceholder = () => (
  <div className="p-8 text-center">
    <h1 className="text-2xl font-bold">About Page (Universal Open)</h1>
  </div>
);

const StatsPagePlaceholder = () => (
  <div className="p-8 text-center">
    <h1 className="text-2xl font-bold">Election Stats (Universal Open)</h1>
  </div>
);

// --- TanStack Query Client ---

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: false,
    },
  },
});

// --- App Root ---

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            {/* ZONE 1: Guest-Only Routes (Non-authenticated) */}
            <Route element={<GuestRoute />}>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register/voter" element={<RegisterVoterPage />} />
              <Route
                path="/register/candidate"
                element={<RegisterCandidatePage />}
              />
            </Route>

            {/* ZONE 2: Universal Open Routes (Auth & Non-Auth) */}
            <Route path="/about" element={<AboutPagePlaceholder />} />
            <Route path="/stats" element={<StatsPagePlaceholder />} />

            {/* ZONE 3: Role-Protected Routes (Authenticated) */}
            <Route element={<RoleRoute allowedRole="Admin" />}>
              <Route path="/admin" element={<AdminPage />} />
            </Route>

            <Route element={<RoleRoute allowedRole="Voter" />}>
              <Route path="/voter" element={<VoterPage />} />
            </Route>

            <Route element={<RoleRoute allowedRole="Candidate" />}>
              <Route path="/candidate" element={<CandidatePage />} />
            </Route>

            {/* Fallbacks */}
            <Route path="/" element={<Navigate to="/stats" replace />} />
            <Route path="*" element={<Navigate to="/stats" replace />} />
          </Routes>
        </BrowserRouter>
        <Toaster position="top-right" />
      </AuthProvider>
    </QueryClientProvider>
  );
}

export default App;
