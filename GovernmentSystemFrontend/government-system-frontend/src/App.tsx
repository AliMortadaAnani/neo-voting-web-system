import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { PublicRoute } from "./components/PublicRoute";
import { AppLayout } from "./components/AppLayout";
import { LoginPage } from "./pages/LoginPage";
import { CitizensListPage } from "./pages/citizens/CitizensListPage";
import { CitizenCreatePage } from "./pages/citizens/CitizenCreatePage";
import { CitizenEditPage } from "./pages/citizens/CitizenEditPage";
import { CitizenDetailsPage } from "./pages/citizens/CitizenDetailsPage";
import { CitizenDeletePage } from "./pages/citizens/CitizenDeletePage";
import { VotersListPage } from "./pages/voters/VotersListPage";
import { VoterEnrollPage } from "./pages/voters/VoterEnrollPage";
import { VoterDetailsPage } from "./pages/voters/VoterDetailsPage";
import { VoterTokenPage } from "./pages/voters/VoterTokenPage";
import { VoterDeletePage } from "./pages/voters/VoterDeletePage";
// Candidates
import { CandidatesListPage } from "./pages/candidates/CandidatesListPage";
import { CandidateNominatePage } from "./pages/candidates/CandidateNominatePage";
import { CandidateDetailsPage } from "./pages/candidates/CandidateDetailsPage";
import { CandidateTokenPage } from "./pages/candidates/CandidateTokenPage";
import { CandidateDeletePage } from "./pages/candidates/CandidateDeletePage";
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
          {/* Public Routes (Accessible only when logged out) */}
          <Route element={<PublicRoute />}>
            <Route path="/login" element={<LoginPage />} />
          </Route>

          {/* Protected Routes (Accessible only when authenticated) */}
          <Route element={<ProtectedRoute />}>
            {/* Authenticated Layout: Navbar, Header, and Page Shell */}
            <Route element={<AppLayout />}>
              <Route path="/" element={<Navigate to="/citizens" replace />} />
              <Route path="/citizens" element={<CitizensListPage />} />
              <Route path="/citizens/create" element={<CitizenCreatePage />} />
              <Route path="/citizens/edit" element={<CitizenEditPage />} />
              <Route
                path="/citizens/details"
                element={<CitizenDetailsPage />}
              />
              <Route path="/citizens/delete" element={<CitizenDeletePage />} />
              <Route path="/voters" element={<VotersListPage />} />
              <Route path="/voters/create" element={<VoterEnrollPage />} />
              <Route path="/voters/details" element={<VoterDetailsPage />} />
              <Route path="/voters/token" element={<VoterTokenPage />} />
              <Route path="/voters/delete" element={<VoterDeletePage />} />
              <Route path="/candidates" element={<CandidatesListPage />} />
              <Route
                path="/candidates/create"
                element={<CandidateNominatePage />}
              />
              <Route
                path="/candidates/details"
                element={<CandidateDetailsPage />}
              />
              <Route
                path="/candidates/token"
                element={<CandidateTokenPage />}
              />
              <Route
                path="/candidates/delete"
                element={<CandidateDeletePage />}
              />
            </Route>
          </Route>

          {/* Fallback for undefined routes */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>

      <Toaster position="top-right" />
    </QueryClientProvider>
  );
}

export default App;
