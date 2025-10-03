import { createContext, useContext, useState } from "react";

/** define the context no data until given something */
const AppContext = createContext(null);

/** provider component, owns the actual state and shares it with everything inside */
export function AppProvider({ children }) {
  // shared auth state for the whole app
  const [username, setUsername] = useState(null);

  
  // This object is what consumers will receive via useApp()
  // It includes both the current value and the function to change it.
  const value = { username, setUsername };


  // put value into the context for all descendants, in this case our value is just our username and setusername 
  // while children is what is wrapped by this tag check app to see where used
  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
}

/**  convenience hook that reads the current context value */
export function useApp() {
  const ctx = useContext(AppContext);
  if (!ctx) throw new Error("useApp must be used inside <AppProvider>");
  return ctx;
}