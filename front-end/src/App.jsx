import "./styles.css";
import { useState } from "react";
import { AppProvider, useApp } from "./context/AppContext";
import LoginPage from "./pages/LoginPage.jsx";
import NavBar from "./components/NavBar.jsx";  // <-- capital B
import GamesPage from "./pages/GamesPage.jsx";
import ProfilePage from "./pages/ProfilePage.jsx";
import AdminPage from "./pages/AdminPage.jsx";

function Shell() {
  const { username, setUsername, role, setRole} = useApp();
  const [route, setRoute] = useState("games");

  if (!username) return (
      <LoginPage
        onLogin={(user) => {
          console.log("✅ Logged in user:", user);
          setUsername(user?.username ?? null);
          setRole(user?.role ?? "user"); // default to user if missing
        }}
      />
    );

  function handleLogout() {
    setUsername(null);
    setRole(null);
    setRoute("games");
  }

  console.log("Current role in App:", role);
  
  let page;
  switch (route) {
    case "games":
      page = <GamesPage onNavigate={setRoute} />;
      break;
    case "profile":
      page = <ProfilePage />;
      break;
    case "admin":
      page = <AdminPage />;
      break;
    default:
      page = <GamesPage />;
  } //depending on route what page are we on

return (
    <>
      <NavBar
        route={route}
        onNavigate={setRoute}
        onLogout={handleLogout}
        role={role}
      />
      <div className="app-container">{page}</div>
    </>
  );
}

export default function App() {
  return (
    <AppProvider>
      <Shell />
    </AppProvider>
  );
}
