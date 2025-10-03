import "./styles.css";
import { useState } from "react";
import { AppProvider, useApp } from "./context/AppContext";
import LoginPage from "./pages/LoginPage.jsx";
import NavBar from "./components/NavBar.jsx";  // <-- capital B
import GamesPage from "./pages/GamesPage.jsx";
import ProfilePage from "./pages/ProfilePage.jsx";

function Shell() {
  const { username, setUsername } = useApp();
  const [route, setRoute] = useState("games");

  if (!username) return <LoginPage onLogin={setUsername} />; //if no user stay on login

  function handleLogout() {
    setUsername(null);
    setRoute("games");
  }

  let page = route === "games" ? <GamesPage /> : <ProfilePage />; //depending on route what page are we on

return (
  <>
    <NavBar route={route} onNavigate={setRoute} onLogout={handleLogout} />
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
