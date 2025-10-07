import { createContext, useContext, useMemo, useState } from "react";

/**
 * AppContext holds global app state like a tiny in-memory session
 * its like a singleton we use to dependency inject everywhere but instead we just have the fields available with this class
 */
const AppContext = createContext(null);

/**
 * AppProvider is the component that OWNS the state and exposes it to
 * everything nested inside via <AppContext.Provider>.
 */
export function AppProvider({ children }) {
  // ---- stubbed fields that we need in profilepage
  const [userId, setUserId] = useState(null);         // e.g., 42
  const [username, setUsername] = useState(null);     // e.g., "tester"
  const [avatarUrl, setAvatarUrl] = useState(null);   // URL for profile image we can maybe add this later with external api
  const [authToken, setAuthToken] = useState(null);   // JWT/access token 

  // Convenience methods 
  const login = (user) => {
    // user: { id, username, avatarUrl, token }
    setUserId(user?.id ?? null);
    setUsername(user?.username ?? null);
    setAvatarUrl(user?.avatarUrl ?? null);
    setAuthToken(user?.token ?? null);
  };

  const logout = () => {
    setUserId(null);
    setUsername(null);
    setAvatarUrl(null);
    setAuthToken(null);
  };

  // we can use this to cache slow to get data
  const value = useMemo(() => ({
    // state
    userId, username, avatarUrl, authToken,
    // setters 
    setUserId, setUsername, setAvatarUrl, setAuthToken,
    // helpers
    login, logout,
  }), [userId, username, avatarUrl, authToken]);

  // Make the value available to all descendants
  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
}

/**
 * Hook that reads the context. Throws if used outside <AppProvider>
 */
export function useApp() {
  const ctx = useContext(AppContext);
  if (ctx === null) throw new Error("useApp must be used inside <AppProvider>");
  return ctx;
}
