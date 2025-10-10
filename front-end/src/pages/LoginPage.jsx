import { useState } from "react";
import snake from "../assets/snake.jpg";
import { UsersAPI } from "../api/users";
import { AuthAPI } from "../api/auth";

export default function LoginPage({ onLogin }) {
  const [username, setUsername] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

//handle login
  async function handleLogin(e) {
    e.preventDefault();

    const trimmed = username.trim();
    if (!trimmed) {
      setError("Please enter a username before logging in.");
      setMessage("");
      return;
    }

    setLoading(true);
    setError("");
    setMessage("");

    try {
      // Send JSON instead of plaintext
      const payload = { username: trimmed };
      console.log("Sending POST /Authentication/LoginAccount with body:", payload);
      await AuthAPI.loginAccount(payload);

      // verify cookie is set
      try {
        await AuthAPI.testAuthorization();
      } catch {
      }

      setMessage("Logged in successfully!");
      

      if (onLogin)
        console.log("Role:", user.role);
        onLogin({
          username: user.username ?? trimmed,
          role: user.role ?? "user",
        });
    } 
    catch (err) {
      console.error("Login error:", err);
      // Show server error msg if there is one
      setError(err?.message || "Login failed.");
    } finally {
      setLoading(false);
    }
  }

//handle requestss
  async function handleRegister() {
    const trimmed = username.trim();
    if (!trimmed) {
      setError("Please enter a username before registering.");
      setMessage("");
      return;
    }

    const confirmed = window.confirm(`You entered "${trimmed}". Is this correct?`);
    if (!confirmed) {
      setMessage("Registration cancelled.");
      setError("");
      return;
    }

    // duplicate user check
    try {
      const existing = await UsersAPI.getByUsername(trimmed);
      if (existing) {
        setError(`Username "${trimmed}" already exists. Please choose another.`);
        setMessage("");
        return;
      }
    } catch {
      // If 404 or error, assume username is available and continue.
    }

    const isAdmin = window.confirm("Are you an admin?");
    const selectedRole = isAdmin ? "admin" : "user";

    setLoading(true);
    setError("");
    setMessage("");

    try {
      //need username and role
      const newUser = { username: trimmed, role: selectedRole };
      await AuthAPI.createAccount(newUser);

      // verify cookie is set by server after create
      try {
        await AuthAPI.testAuthorization();
      } catch {
      }

      setMessage(`Account created for "${trimmed}"!`);
      if (onLogin) onLogin({ username: trimmed });
    } catch (err) {
      console.error("Register error:", err);
      // If backend sends error message, throw it here
      setError(err?.message || `Could not create user "${trimmed}".`);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="center-screen">
      <form onSubmit={handleLogin} className="form-auth">
        <h1 style={{ margin: 0 }}>SGDB</h1>

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

        <button type="submit" className="btn" disabled={loading}>
          {loading ? "Loading..." : "Login"}
        </button>

        <button type="button" className="btn" onClick={handleRegister} disabled={loading}>
          Register
        </button>

        {message && <p className="success-text">{message}</p>}
        {error && <p className="error-text">{error}</p>}
      </form>
    </div>
  );
}
