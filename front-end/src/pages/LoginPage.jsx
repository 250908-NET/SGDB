import { useState } from "react";
import snake from "../assets/snake.jpg";
import { UsersAPI } from "../api/users";
import { AuthAPI } from "../api/auth";

export default function LoginPage({ onLogin }) {
  const [username, setUsername] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");


  // LOGIN HANDLER -------------
  async function handleLogin(e) {
    e.preventDefault();
    let trimmed = username.trim();
    console.log("Attempting login with username:", trimmed);

    if (trimmed == "") {
      console.warn("Username empty, aborting login");
      setError("Please enter a username before logging in.");
      setMessage("");      // clear any previous messages
      return;
    }

    setLoading(true);
    setError("");

    try {
      console.log("Sending POST /authentication/LoginAccount with body:", { username: trimmed });
      // Call backend: GET /api/user/username/{username}
      // let user1 = await UsersAPI.getByUsername(trimmed);
      // console.log("User found:", user1);
      
      let user = await AuthAPI.loginAccount({ username: trimmed });
      console.log("User found:", user);

      setMessage("Logged in successfully!");
      

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

  // REGISTER HANDLER -------------
  async function handleRegister() {
    let trimmed = username.trim();
    if (!trimmed) {
      console.log("Blank username");
      setError("Please enter a username before registering.");
      return;
    }

    // Confirm username
    const confirmUsername = window.confirm(`You entered "${trimmed}". Is this correct?`);

    if (!confirmUsername) {
      console.log("Registration cancelled.");
      setMessage("Registration cancelled.");
      return;
    }

    // Check if username exists
    try {
        const existingUser = await UsersAPI.getByUsername(trimmed);
        if (existingUser) {
          console.warn("Duplicate username.");
          setError(`Username "${trimmed}" already exists. Please choose another.`);
          setLoading(false);
          return;
        }
    }
    
    catch (err) {
      console.log("Username available, proceeding to registration.");
    }

    // Ask if admin
    let isAdmin = window.confirm("Are you an admin?");
    let selectedRole = isAdmin ? "admin" : "user";

    console.log(`Selected role: ${selectedRole}`);

    setLoading(true);
    setError("");
    setMessage("");

    try {
      // Call backend: POST /api/user

      let newUserData = {
        username: trimmed,
        role: selectedRole,         
        userGenres: [],        
        gameLibrary: [],      
      }

      let result = await AuthAPI.createAccount(newUserData);
      console.log("User registered:", trimmed);

      setMessage(`Account created for "${result.username}"!`);
      if (onLogin) onLogin(result); 
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
