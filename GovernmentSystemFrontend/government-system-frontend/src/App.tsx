import { Routes, Route, Navigate } from "react-router-dom";

// Temporary placeholder components until we build the actual pages
const LoginPage = () => (
  <div className="p-6 text-xl font-bold">Login Page(Public)</div>
);
const HomePage = () => (
  <div className="p-6 text-xl font-bold">Home Page (Protected)</div>
);
const NotFoundPage = () => (
  <div className="p-6 text-xl font-bold text-red-500">404 Not Found</div>
);

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/" element={<HomePage />} />
      <Route path="/home" element={<Navigate to="/" replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;
