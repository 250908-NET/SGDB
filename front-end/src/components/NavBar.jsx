// src/components/NavBar.jsx
export default function NavBar({ route, onNavigate, onLogout }) {
  return (
    <nav className="app-navbar" aria-label="Primary">
      <button
        className="app-nav-btn"
        onClick={() => onNavigate("profile")}
        aria-current={route === "profile" ? "page" : undefined}
      >
        Profile
      </button>
      <button
        className="app-nav-btn"
        onClick={() => onNavigate("games")}
        aria-current={route === "games" ? "page" : undefined}
      >
        Games
      </button>
      <button className="app-nav-btn" onClick={onLogout}>Logout</button>
    </nav>
  );
}
