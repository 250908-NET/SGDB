// src/components/NavBar.jsx
export default function NavBar({ route, onNavigate, onLogout, role }) {
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

      {role === "admin" && (
        <button
          className="app-nav-btn"
          onClick={() => onNavigate("admin")}
          aria-current={route === "admin" ? "page" : undefined}
        >
          Admin
        </button>
      )}

      <button className="app-nav-btn" onClick={onLogout}>Logout</button>
    </nav>
  );
}
