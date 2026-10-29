import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";

// Route guards
import { ProtectedRoute } from "./components/general/ProtectedRoute";
import { PublicRoute } from "./components/general/PublicRoute";

// page imports
import { LoginPage } from "./pages/general/LoginPage";
import { CitizenPage } from "./pages/citizens/CitizenPage";
import { CitizenListPage } from "./pages/citizens/CitizenListPage";
import { CitizenSearchPage } from "./pages/citizens/CitizenSearchPage";
import { CitizenRegisterPage } from "./pages/citizens/CitizenRegisterPage";
import { CitizenEditPage } from "./pages/citizens/CitizenEditPage";
import { CitizenDeletePage } from "./pages/citizens/CitizenDeletePage";
import { VoterPage } from "./pages/voters/VoterPage";
import { VoterListPage } from "./pages/voters/VoterListPage";
import { VoterSearchPage } from "./pages/voters/VoterSearchPage";
import { VoterRegisterPage } from "./pages/voters/VoterRegisterPage";
import { VoterTokenPage } from "./pages/voters/VoterTokenPage";
import { VoterDeletePage } from "./pages/voters/VoterDeletePage";
import { CandidatePage } from "./pages/candidates/CandidatePage";
import { CandidateListPage } from "./pages/candidates/CandidateListPage";
import { CandidateSearchPage } from "./pages/candidates/CandidateSearchPage";
import { CandidateRegisterPage } from "./pages/candidates/CandidateRegisterPage";
import { CandidateTokenPage } from "./pages/candidates/CandidateTokenPage";
import { CandidateDeletePage } from "./pages/candidates/CandidateDeletePage";

import { AboutPage } from "./pages/general/AboutPage";

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

          <Route element={<ProtectedRoute />}>
            <Route path="/" element={<Navigate to="/citizens" replace />} />

            {/* Citizens */}
            <Route path="/citizens" element={<CitizenPage />} />
            <Route path="/citizens/list" element={<CitizenListPage />} />
            <Route path="/citizens/details" element={<CitizenSearchPage />} />
            <Route
              path="/citizens/register"
              element={<CitizenRegisterPage />}
            />
            <Route path="/citizens/edit" element={<CitizenEditPage />} />
            <Route path="/citizens/delete" element={<CitizenDeletePage />} />

            {/* Voters */}
            <Route path="/voters" element={<VoterPage />} />
            <Route path="/voters/list" element={<VoterListPage />} />
            <Route path="/voters/details" element={<VoterSearchPage />} />
            <Route path="/voters/register" element={<VoterRegisterPage />} />
            <Route path="/voters/new-token" element={<VoterTokenPage />} />
            <Route path="/voters/delete" element={<VoterDeletePage />} />

            {/* Candidates */}
            <Route path="/candidates" element={<CandidatePage />} />
            <Route path="/candidates/list" element={<CandidateListPage />} />
            <Route
              path="/candidates/details"
              element={<CandidateSearchPage />}
            />
            <Route
              path="/candidates/register"
              element={<CandidateRegisterPage />}
            />
            <Route
              path="/candidates/new-token"
              element={<CandidateTokenPage />}
            />
            <Route
              path="/candidates/delete"
              element={<CandidateDeletePage />}
            />
          </Route>

          {/* Catch-all redirect */}
          <Route path="*" element={<Navigate to="/citizens" replace />} />
        </Routes>
      </BrowserRouter>

      <Toaster
        position="top-right"
        toastOptions={{
          duration: 1000, // default to 1 second for all
        }}
      />
    </QueryClientProvider>
  );
}

export default App;
