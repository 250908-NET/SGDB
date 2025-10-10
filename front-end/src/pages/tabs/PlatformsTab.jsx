import { useState } from "react";
import { PlatformsAPI } from "../../api/platforms";

export default function PlatformsTab({ platforms, reload }) {
  const [selectedPlatform, setSelectedPlatform] = useState(null);
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
      if (selectedPlatform) {
        await PlatformsAPI.update(selectedPlatform.platformId, form);
      } else {
        await PlatformsAPI.create(form);
      }
      await reload();
      resetForm();
    } catch (err) {
      console.error(err);
      setError("Failed to save platform.");
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to delete this platform?")) return;
    try {
      setLoading(true);
      await PlatformsAPI.remove(id);
      await reload();
    } catch (err) {
      console.error(err);
      setError("Failed to delete platform.");
    } finally {
      setLoading(false);
    }
  }

  function handleSelect(p) {
    setSelectedPlatform(p);
    setForm({ name: p.name });
  }

  function resetForm() {
    setSelectedPlatform(null);
    setForm({ name: "" });
    setError("");
  }

  return (
    <div>
      {/* ---------- Platform Form ---------- */}
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
        <h3>{selectedPlatform ? "Update Platform" : "Create New Platform"}</h3>

        {error && <div style={{ color: "red" }}>{error}</div>}
        {loading && <div style={{ color: "#555" }}>Loading...</div>}

        <input
          type="text"
          name="name"
          value={form.name}
          onChange={handleChange}
          placeholder="Platform Name"
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
            background: selectedPlatform ? "#10b981" : "#2563eb",
            color: "white",
            padding: "10px 14px",
            border: "none",
            borderRadius: 6,
            cursor: "pointer",
          }}
        >
          {selectedPlatform ? "Update Platform" : "Add Platform"}
        </button>

        {selectedPlatform && (
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

      {/* ---------- Platform List ---------- */}
      <h3>Existing Platforms</h3>
      <div style={{ display: "grid", gap: 8 }}>
        {platforms.map((p) => (
          <div
            key={p.platformId}
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
              <strong>{p.name}</strong>
            </div>
            <div style={{ display: "flex", gap: 8 }}>
              <button
                onClick={() => handleSelect(p)}
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
                onClick={() => handleDelete(p.platformId)}
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

        {platforms.length === 0 && !loading && (
          <div style={{ color: "#555", padding: 10 }}>No platforms found.</div>
        )}
      </div>
    </div>
  );
}
