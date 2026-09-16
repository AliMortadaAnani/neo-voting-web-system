import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";

// 100% manual control: No silent retries, no automatic refetching on alt-tab
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
      <main className="min-h-screen bg-slate-50 p-8 text-slate-900">
        <h1 className="text-2xl font-bold">Government System Portal</h1>
        <p className="text-slate-600 mt-2">Foundation and providers ready.</p>
      </main>

      {/* Global toast notification system */}
      <Toaster position="top-right" />
    </QueryClientProvider>
  );
}

export default App;
