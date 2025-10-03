import { useApp } from "../context/AppContext";

export default function HomePage() {
  const { username, setUsername } = useApp(); //This can probably be removed but lets keep it for now since we will need more pages for adding to lists

  return (
    <div style={{ padding: 16 }}>
      <h2>Welcome, {username}</h2>
      <p>This is a template page until login and profile pages exist.</p>
      <button onClick={() => setUsername(null)}>Logout</button>
    </div>
  );
}