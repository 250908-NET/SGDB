import { useState } from "react";
import { GamesAPI } from "../../api/games";

export default function GamesTab({ games, companies, genres, platforms, reload }) {
  const [selectedGame, setSelectedGame] = useState(null);
  const [form, setForm] = useState({
    name: "",
    releaseDate: "",
    publisherId: "",
    developerId: "",
    genres: [],
    platforms: [],
  });

  const [publisherFilter, setPublisherFilter] = useState("");
  const [developerFilter, setDeveloperFilter] = useState("");
  const [genreFilter, setGenreFilter] = useState("");
  const [platformFilter, setPlatformFilter] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  function handleInputChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  function handleCheckboxChange(e, field) {
    const value = e.target.value;
    setForm((prev) => {
      const arr = new Set(prev[field]);
      if (arr.has(value)) arr.delete(value);
      else arr.add(value);
      return { ...prev, [field]: Array.from(arr) };
    });
  }

  async function handleCreateOrUpdate(e) {
    e.preventDefault();
    try {
      setLoading(true);
      const payload = {
        name: form.name,
        releaseDate: form.releaseDate,
        publisherId: parseInt(form.publisherId),
        developerId: parseInt(form.developerId),
        platformIds: form.platforms.map((id) => parseInt(id)),
        genreIds: form.genres.map((id) => parseInt(id)),
      };

      if (selectedGame) {
        await GamesAPI.update(selectedGame.gameId, payload);
      } else {
        await GamesAPI.create(payload);
      }

      await reload();
      resetForm();
      setSelectedGame(null);
    } catch (err) {
      console.error("Failed to save game:", err);
      setError("Failed to save game.");
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to delete this game?")) return;

    try {
      setLoading(true);
      await GamesAPI.remove(id);
      await reload();
      alert("Game deleted successfully!");
    } catch (err) {
      console.error("Failed to delete game:", err);
      alert("Error deleting game — please check if it’s referenced by other data.");
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
      genres: game.genres || [],
      platforms: game.platforms || [],
    });
  }

  function resetForm() {
    setForm({
      name: "",
      releaseDate: "",
      publisherId: "",
      developerId: "",
      genres: [],
      platforms: [],
    });
  }

  function getNames(ids, list, idKey, nameKey) {
    return ids
      .map((id) => list.find((x) => String(x[idKey]) === String(id))?.[nameKey])
      .filter(Boolean)
      .join(", ");
  }

  return (
    <div>
      <form
        onSubmit={handleCreateOrUpdate}
        style={{
          display: "grid",
          gap: 12,
          marginBottom: 20,
          padding: 16,
          border: "1px solid #ddd",
          borderRadius: 8,
          background: "#fafafa",
        }}
      >
        <h3>{selectedGame ? "Update Game" : "Create New Game"}</h3>
        {error && <div style={{ color: "red" }}>{error}</div>}

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

        {/* Publisher */}
        <div>
          <label style={{ fontWeight: 600 }}>Publisher</label>
          <input
            type="text"
            placeholder="Search publishers..."
            onChange={(e) => setPublisherFilter(e.target.value.toLowerCase())}
            style={{
              width: "98%",
              padding: "6px 10px",
              borderRadius: 6,
              border: "1px solid #ccc",
              marginBottom: 6,
            }}
          />
          <div
            style={{
              border: "1px solid #ccc",
              borderRadius: 6,
              padding: 8,
              maxHeight: 120,
              overflowY: "auto",
              background: "white",
            }}
          >
            {companies
              .filter((c) => c.name.toLowerCase().includes(publisherFilter))
              .map((c) => (
                <label key={c.companyId} style={{ display: "block", fontSize: 14 }}>
                  <input
                    type="radio"
                    name="publisherId"
                    value={String(c.companyId)}
                    checked={String(form.publisherId) === String(c.companyId)}
                    onChange={handleInputChange}
                    style={{ marginRight: 6 }}
                  />
                  {c.name}
                </label>
              ))}
          </div>
        </div>

        {/* Developer */}
        <div>
          <label style={{ fontWeight: 600 }}>Developer</label>
          <input
            type="text"
            placeholder="Search developers..."
            onChange={(e) => setDeveloperFilter(e.target.value.toLowerCase())}
            style={{
              width: "98%",
              padding: "6px 10px",
              borderRadius: 6,
              border: "1px solid #ccc",
              marginBottom: 6,
            }}
          />
          <div
            style={{
              border: "1px solid #ccc",
              borderRadius: 6,
              padding: 8,
              maxHeight: 120,
              overflowY: "auto",
              background: "white",
            }}
          >
            {companies
              .filter((c) => c.name.toLowerCase().includes(developerFilter))
              .map((c) => (
                <label key={c.companyId} style={{ display: "block", fontSize: 14 }}>
                  <input
                    type="radio"
                    name="developerId"
                    value={String(c.companyId)}
                    checked={String(form.developerId) === String(c.companyId)}
                    onChange={handleInputChange}
                    style={{ marginRight: 6 }}
                  />
                  {c.name}
                </label>
              ))}
          </div>
        </div>

        {/* Genres */}
        <div>
          <label style={{ fontWeight: 600 }}>Genres</label>
          <input
            type="text"
            placeholder="Search genres..."
            onChange={(e) => setGenreFilter(e.target.value.toLowerCase())}
            style={{
              width: "98%",
              padding: "6px 10px",
              borderRadius: 6,
              border: "1px solid #ccc",
              marginBottom: 6,
            }}
          />
          <div
            style={{
              border: "1px solid #ccc",
              borderRadius: 6,
              padding: 8,
              maxHeight: 150,
              overflowY: "auto",
              background: "white",
            }}
          >
            {genres
              .filter((g) => g.name.toLowerCase().includes(genreFilter))
              .map((g) => (
                <label key={g.genreId} style={{ display: "block", fontSize: 14 }}>
                  <input
                    type="checkbox"
                    value={String(g.genreId)}
                    checked={form.genres.includes(String(g.genreId))}
                    onChange={(e) => handleCheckboxChange(e, "genres")}
                    style={{ marginRight: 6 }}
                  />
                  {g.name}
                </label>
              ))}
          </div>
        </div>

        {/* Platforms */}
        <div>
          <label style={{ fontWeight: 600 }}>Platforms</label>
          <input
            type="text"
            placeholder="Search platforms..."
            onChange={(e) => setPlatformFilter(e.target.value.toLowerCase())}
            style={{
              width: "98%",
              padding: "6px 10px",
              borderRadius: 6,
              border: "1px solid #ccc",
              marginBottom: 6,
            }}
          />
          <div
            style={{
              border: "1px solid #ccc",
              borderRadius: 6,
              padding: 8,
              maxHeight: 150,
              overflowY: "auto",
              background: "white",
            }}
          >
            {platforms
              .filter((p) => p.name.toLowerCase().includes(platformFilter))
              .map((p) => (
                <label key={p.platformId} style={{ display: "block", fontSize: 14 }}>
                  <input
                    type="checkbox"
                    value={String(p.platformId)}
                    checked={form.platforms.includes(String(p.platformId))}
                    onChange={(e) => handleCheckboxChange(e, "platforms")}
                    style={{ marginRight: 6 }}
                  />
                  {p.name}
                </label>
              ))}
          </div>
        </div>

        <button
          type="submit"
          disabled={loading}
          style={{
            background: selectedGame ? "#10b981" : "#2563eb",
            color: "white",
            padding: "10px 14px",
            border: "none",
            borderRadius: 6,
            cursor: loading ? "not-allowed" : "pointer",
            opacity: loading ? 0.7 : 1,
          }}
        >
          {loading
            ? selectedGame
              ? "Updating..."
              : "Adding..."
            : selectedGame
            ? "Update Game"
          : "Add Game"}
      </button>

{loading && (
  <p style={{ color: "#6b7280", fontStyle: "italic" }}>
    Please wait — processing your request...
  </p>
)}


        {selectedGame && (
          <button
            type="button"
            onClick={resetForm}
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

      {/* Game List */}
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
              <small>{new Date(g.releaseDate).toLocaleDateString()}</small> <br />
              <small>Platforms: {getNames(g.platforms, platforms, "platformId", "name")}</small> <br />
              <small>Genres: {getNames(g.genres, genres, "genreId", "name")}</small>
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
      </div>
    </div>
  );
}
