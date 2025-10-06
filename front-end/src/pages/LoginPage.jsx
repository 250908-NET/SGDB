import { useState } from "react";
import snake from "../assets/snake.jpg"; // <-- your image (Vite turns this into a URL)

export default function LoginPage({ onLogin }) {
  const [username, setUsername] = useState("");

  function handleLogin(e) {
    e.preventDefault();
    const trimmed = username.trim();
    if (!trimmed) return;
    if (onLogin) onLogin(trimmed);
  }

  return (
    <div className="center-screen">
      <form onSubmit={handleLogin} className="form-auth">
        <h1 style={{ margin: 0 }}>Gaming</h1>

        {}
        <img className="logo" src={snake} alt="App logo" />

        <label htmlFor="username" className="label">Username</label>
        <input
          id="username"
          className="input"
          placeholder="enter username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          autoFocus
        />

        <button type="submit" className="btn">Login</button>
        <button
          type="button"
          className="btn"
          onClick={() => alert("Register flow coming soon")}
        >
          Register
        </button>
      </form>
    </div>
  );
}
