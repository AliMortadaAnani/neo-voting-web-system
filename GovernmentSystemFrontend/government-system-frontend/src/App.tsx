import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";

// Route guards
import { ProtectedRoute } from "./components/ProtectedRoute";
import { PublicRoute } from "./components/PublicRoute";

// Minimal page imports
import { LoginPage } from "./pages/LoginPage";
import { CitizenPage } from "./pages/CitizenPage";
import { VoterPage } from "./pages/VoterPage";
import { CandidatePage } from "./pages/CandidatePage";

import { AboutPage } from "./pages/AboutPage";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: false,
    },
  },
});

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/about" element={<AboutPage />} />
          {/* Public Routes */}
          <Route element={<PublicRoute />}>
            <Route path="/login" element={<LoginPage />} />
          </Route>

          {/* Protected Routes */}
          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<Navigate to="/citizens" replace />} />
            <Route path="/citizens" element={<CitizenPage />}>
              <Route path="/citizens/list" element={<CitizenPage />} />
              <Route path="/citizens/details" element={<CitizenPage />} />
              <Route path="/citizens/register" element={<CitizenPage />} />
              <Route path="/citizens/edit" element={<CitizenPage />} />
              <Route path="/citizens/delete" element={<CitizenPage />} />
            </Route>
            <Route path="/voters" element={<VoterPage />}>
              <Route path="/voters/list" element={<VoterPage />} />
              <Route path="/voters/details" element={<VoterPage />} />
              <Route path="/voters/register" element={<VoterPage />} />
              <Route path="/voters/new-token" element={<VoterPage />} />
              <Route path="/voters/delete" element={<VoterPage />} />
            </Route>
            <Route path="/candidates" element={<CandidatePage />}>
              <Route path="/candidates/list" element={<CandidatePage />} />
              <Route path="/candidates/details" element={<CandidatePage />} />
              <Route path="/candidates/register" element={<CandidatePage />} />
              <Route path="/candidates/new-token" element={<CandidatePage />} />
              <Route path="/candidates/delete" element={<CandidatePage />} />
            </Route>
          </Route>

          {/* Catch-all redirect */}
          <Route path="*" element={<Navigate to="/citizens" replace />} />
        </Routes>
      </BrowserRouter>

      <Toaster position="top-right" />
    </QueryClientProvider>
  );
}

export default App;
