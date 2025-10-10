import { useState } from "react";
import { GenresAPI } from "../../api/genres";

export default function GenresTab({ genres, reload }) {
  const [selectedGenre, setSelectedGenre] = useState(null);
  const [form, setForm] = useState({ name: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      setLoading(true);
      if (selectedGenre) {
        await GenresAPI.update(selectedGenre.genreId, form);
      } else {
        await GenresAPI.create(form);
      }
      await reload();
      resetForm();
    } catch (err) {
      console.error(err);
      setError("Failed to save genre.");
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to delete this genre?")) return;
    try {
      setLoading(true);
      await GenresAPI.remove(id);
      await reload();
    } catch (err) {
      console.error(err);
      setError("Failed to delete genre.");
    } finally {
      setLoading(false);
    }
  }

  function handleSelect(genre) {
    setSelectedGenre(genre);
    setForm({ name: genre.name });
  }

  function resetForm() {
    setSelectedGenre(null);
    setForm({ name: "" });
    setError("");
  }

  return (
    <div>
      {/* ---------- Genre Form ---------- */}
      <form
        onSubmit={handleSubmit}
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
        <h3>{selectedGenre ? "Update Genre" : "Create New Genre"}</h3>

        {error && <div style={{ color: "red" }}>{error}</div>}
        {loading && <div style={{ color: "#555" }}>Loading...</div>}

        <input
          type="text"
          name="name"
          value={form.name}
          onChange={handleChange}
          placeholder="Genre Name"
          required
          style={{
            padding: "8px 10px",
            borderRadius: 6,
            border: "1px solid #ccc",
          }}
        />

        <button
          type="submit"
          style={{
            background: selectedGenre ? "#10b981" : "#2563eb",
            color: "white",
            padding: "10px 14px",
            border: "none",
            borderRadius: 6,
            cursor: "pointer",
          }}
        >
          {selectedGenre ? "Update Genre" : "Add Genre"}
        </button>

        {selectedGenre && (
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

      {/* ---------- Genre List ---------- */}
      <h3>Existing Genres</h3>
      <div style={{ display: "grid", gap: 8 }}>
        {genres.map((g) => (
          <div
            key={g.genreId}
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
              <strong>{g.name}</strong>
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
                onClick={() => handleDelete(g.genreId)}
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

        {genres.length === 0 && !loading && (
          <div style={{ color: "#555", padding: 10 }}>No genres found.</div>
        )}
      </div>
    </div>
  );
}
