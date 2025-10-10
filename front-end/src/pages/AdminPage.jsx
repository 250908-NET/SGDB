import { useEffect, useState } from "react";
import { GamesAPI } from "../api/games";
import { CompaniesAPI } from "../api/companies";

export default function AdminPage() {
  const [games, setGames] = useState([]);
  const [companies, setCompanies] = useState([]);
  const [selectedGame, setSelectedGame] = useState(null);
  const [form, setForm] = useState({
    name: "",
    releaseDate: "",
    publisherId: "",
    developerId: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Load games and companies
  useEffect(() => {
    loadGames();
    loadCompanies();
  }, []);

  async function loadGames() {
    try {
      setLoading(true);
      const data = await GamesAPI.getAll();
      setGames(data || []);
      setError("");
    } catch (err) {
      console.error(err);
      setError("Failed to load games.");
    } finally {
      setLoading(false);
    }
  }

  async function loadCompanies() {
    try {
      const data = await CompaniesAPI.getAll();
      setCompanies(data || []);
    } catch (err) {
      console.error("Failed to load companies:", err);
    }
  }

  function handleInputChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  async function handleCreate(e) {
    e.preventDefault();
    try {
      setLoading(true);
      await GamesAPI.create(form);
      await loadGames();
      resetForm();
    } catch (err) {
      console.error(err);
      setError("Failed to create game.");
    } finally {
      setLoading(false);
    }
  }

  async function handleUpdate(e) {
    e.preventDefault();
    if (!selectedGame) return;
    try {
      setLoading(true);
      await GamesAPI.update(selectedGame.gameId, form);
      await loadGames();
      resetForm();
      setSelectedGame(null);
    } catch (err) {
      console.error(err);
      setError("Failed to update game.");
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(gameId) {
    if (!window.confirm("Are you sure you want to delete this game?")) return;
    try {
      setLoading(true);
      await GamesAPI.remove(gameId);
      await loadGames();
    } catch (err) {
      console.error(err);
      setError("Failed to delete game.");
    } finally {
      setLoading(false);
    }
  }

  function handleSelect(game) {
    setSelectedGame(game);
    setForm({
      name: game.name || "",
      releaseDate: game.releaseDate ? game.releaseDate.split("T")[0] : "",
      publisherId: game.publisherId || "",
      developerId: game.developerId || "",
    });
  }

  function resetForm() {
    setForm({ name: "", releaseDate: "", publisherId: "", developerId: "" });
  }

  return (
    <div style={{ padding: 24, maxWidth: 900, margin: "0 auto" }}>
      <h2>Admin Dashboard</h2>
      <p>Manage games here (Create, Update, Delete).</p>

      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}
      {loading && <div style={{ color: "#555" }}>Loading...</div>}

      {/* ---------- Game Form ---------- */}
      <form
        onSubmit={selectedGame ? handleUpdate : handleCreate}
        style={{
          display: "grid",
          gap: 10,
          marginBottom: 20,
          padding: 16,
          border: "1px solid #ddd",
          borderRadius: 8,
          background: "#fafafa",
        }}
      >
        <h3>{selectedGame ? "Update Game" : "Create New Game"}</h3>

        <input
          type="text"
          name="name"
          value={form.name}
          onChange={handleInputChange}
          placeholder="Game Name"
          required
          style={{ padding: "8px 10px", borderRadius: 6, border: "1px solid #ccc" }}
        />
        <input
          type="date"
          name="releaseDate"
          value={form.releaseDate}
          onChange={handleInputChange}
          required
          style={{ padding: "8px 10px", borderRadius: 6, border: "1px solid #ccc" }}
        />

        {/* Publisher Dropdown */}
        <label>
          Publisher:
          <select
            name="publisherId"
            value={form.publisherId}
            onChange={handleInputChange}
            style={{
              width: "100%",
              padding: "8px 10px",
              borderRadius: 6,
              border: "1px solid #ccc",
              marginTop: 4,
            }}
          >
            <option value="">-- Select Publisher --</option>
            {companies.map((c) => (
              <option key={c.companyId} value={c.companyId}>
                {c.name}
              </option>
            ))}
          </select>
        </label>

        {/* Developer Dropdown */}
        <label>
          Developer:
          <select
            name="developerId"
            value={form.developerId}
            onChange={handleInputChange}
            style={{
              width: "100%",
              padding: "8px 10px",
              borderRadius: 6,
              border: "1px solid #ccc",
              marginTop: 4,
            }}
          >
            <option value="">-- Select Developer --</option>
            {companies.map((c) => (
              <option key={c.companyId} value={c.companyId}>
                {c.name}
              </option>
            ))}
          </select>
        </label>

        <button
          type="submit"
          style={{
            background: selectedGame ? "#10b981" : "#2563eb",
            color: "white",
            padding: "10px 14px",
            border: "none",
            borderRadius: 6,
            cursor: "pointer",
          }}
        >
          {selectedGame ? "Update Game" : "Add Game"}
        </button>

        {selectedGame && (
          <button
            type="button"
            onClick={() => {
              setSelectedGame(null);
              resetForm();
            }}
            style={{
              background: "#6b7280",
              color: "white",
              padding: "8px 14px",
              border: "none",
              borderRadius: 6,
              cursor: "pointer",
            }}
          >
            Cancel Edit
          </button>
        )}
      </form>

      {/* ---------- Game List ---------- */}
      <h3>Existing Games</h3>
      <div style={{ display: "grid", gap: 8 }}>
        {games.map((g) => (
          <div
            key={g.gameId}
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              padding: 10,
              border: "1px solid #ddd",
              borderRadius: 6,
              background: "#fff",
            }}
          >
            <div>
              <strong>{g.name}</strong> <br />
              <small>{new Date(g.releaseDate).toLocaleDateString()}</small>
            </div>
            <div style={{ display: "flex", gap: 8 }}>
              <button
                onClick={() => handleSelect(g)}
                style={{
                  background: "#f59e0b",
                  color: "white",
                  border: "none",
                  borderRadius: 6,
                  padding: "6px 10px",
                  cursor: "pointer",
                }}
              >
                Edit
              </button>
              <button
                onClick={() => handleDelete(g.gameId)}
                style={{
                  background: "#dc2626",
                  color: "white",
                  border: "none",
                  borderRadius: 6,
                  padding: "6px 10px",
                  cursor: "pointer",
                }}
              >
                Delete
              </button>
            </div>
          </div>
        ))}

        {games.length === 0 && !loading && (
          <div style={{ color: "#777", padding: 10 }}>No games found.</div>
        )}
      </div>
    </div>
  );
}
