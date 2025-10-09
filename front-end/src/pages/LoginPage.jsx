import { useState } from "react";
import snake from "../assets/snake.jpg";
import { UsersAPI } from "../api/users"; // Import your API functions

export default function LoginPage({ onLogin }) {
  const [username, setUsername] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function handleLogin(e) {
    e.preventDefault();
    const trimmed = username.trim();
    if (!trimmed) return;

    setLoading(true);
    setError("");

    try {
      // Call backend: GET /api/user/username/{username}
      const user = await UsersAPI.getByUsername(trimmed);
      console.log("User found:", user);

      if (onLogin) onLogin(user); // Pass user data up to parent (App)
    } 
    catch (err) {
      console.error("Login error:", err);
      setError(`User "${trimmed}" not found.`);
    } 
    finally {
      setLoading(false);
    }
  }

  async function handleRegister() {
    

    let trimmed = username.trim();
    if (!trimmed) {
      setError("Please enter a username before registering.");
      return;
    }

    // Confirm username
    const confirmUsername = window.confirm(
      `You entered "${trimmed}". Is this correct?`
    );
    if (!confirmUsername) {
      setMessage("Registration canceled.");
      return;
    }

    // Check if username exists
    try {
        const existingUser = await UsersAPI.getByUsername(trimmed);
        if (existingUser) {
          setError(`Username "${trimmed}" already exists. Please choose another.`);
          setLoading(false);
          return;
        }
    }
    
    catch (err) {
      console.log("Username available, proceeding to registration.");
    }

    // Ask if admin
    const isAdmin = window.confirm("Are you an admin?");
    const selectedRole = isAdmin ? "admin" : "user";


    setLoading(true);
    setError("");
    setMessage("");

    try {
      // Call backend: POST /api/user

      let newUserData = {
        username: trimmed,
        role: selectedRole,          // default role
        userGenres: [],        // empty for now
        gameLibrary: [],       // empty for now
      }

      let newUser = await UsersAPI.create(newUserData);
      console.log("User registered:", newUser);

      setMessage(`Account created for "${newUser.username}"!`);
      if (onLogin) onLogin(newUser); // optional: auto-login after register
    } 
    catch (err) {
      console.error("Register error:", err);
      setError(`Could not create user "${trimmed}".`);
    } 
    finally {
      setLoading(false);
    }
  }

  return (
    <div className="center-screen">
      <form onSubmit={handleLogin} className="form-auth">
        <h1 style={{ margin: 0 }}>Gaming</h1>

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

        <button
          type="button"
          className="btn"
          onClick={handleRegister}
        >
          Register
        </button>

        {error && <p className="error-text">{error}</p>}
      </form>
    </div>
  );
}
