import { useEffect, useState } from "react";
import { CompaniesAPI } from "../../api/companies";
import { GamesAPI } from "../../api/games";

export default function CompaniesTab({ companies, reload }) {
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [form, setForm] = useState({ name: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [gameNames, setGameNames] = useState({}); // cache of { gameId: gameName }

  useEffect(() => {
    loadGameNames();
  }, [companies]);

  async function loadGameNames() {
    if (!companies || companies.length === 0) return;

    const allIds = new Set();
    companies.forEach((c) => {
      (c.developedGames || []).forEach((id) => allIds.add(id));
      (c.publishedGames || []).forEach((id) => allIds.add(id));
    });

    if (allIds.size === 0) return;

    const nameCache = {};
    await Promise.all(
      Array.from(allIds).map(async (id) => {
        try {
          const game = await GamesAPI.getById(id);
          nameCache[id] = game?.name || `Game #${id}`;
        } catch {
          nameCache[id] = `Game #${id}`;
        }
      })
    );
    setGameNames(nameCache);
  }

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      setLoading(true);
      if (selectedCompany) {
        await CompaniesAPI.update(selectedCompany.companyId, form);
      } else {
        await CompaniesAPI.create(form);
      }
      await reload();
      resetForm();
    } catch (err) {
      console.error(err);
      setError("Failed to save company.");
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Are you sure you want to delete this company?")) return;
    try {
      setLoading(true);
      await CompaniesAPI.remove(id);
      await reload();
    } catch (err) {
      console.error(err);
      setError("Failed to delete company.");
    } finally {
      setLoading(false);
    }
  }

  function handleSelect(c) {
    setSelectedCompany(c);
    setForm({ name: c.name });
  }

  function resetForm() {
    setSelectedCompany(null);
    setForm({ name: "" });
    setError("");
  }

  return (
    <div>
      {/* ---------- Company Form ---------- */}
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
        <h3>{selectedCompany ? "Update Company" : "Create New Company"}</h3>

        {error && <div style={{ color: "red" }}>{error}</div>}
        {loading && <div style={{ color: "#555" }}>Loading...</div>}

        <input
          type="text"
          name="name"
          value={form.name}
          onChange={handleChange}
          placeholder="Company Name"
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
            background: selectedCompany ? "#10b981" : "#2563eb",
            color: "white",
            padding: "10px 14px",
            border: "none",
            borderRadius: 6,
            cursor: "pointer",
          }}
        >
          {selectedCompany ? "Update Company" : "Add Company"}
        </button>

        {selectedCompany && (
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

      {/* ---------- Company List ---------- */}
      <h3>Existing Companies</h3>
      <div style={{ display: "grid", gap: 8 }}>
        {companies.map((c) => {
          const developed = c.developedGames || [];
          const published = c.publishedGames || [];
          const hasAnyGames = developed.length > 0 || published.length > 0;

          return (
            <div
              key={c.companyId}
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
                <strong>{c.name}</strong>
                {(developed.length > 0 || published.length > 0) && (
                  <div style={{ marginTop: 6, fontSize: 13, color: "#333" }}>
                    {developed.length > 0 && (
                      <div>
                        <b>Developed:</b>{" "}
                        {developed
                          .map((id) => gameNames[id] || `#${id}`)
                          .join(", ")}
                      </div>
                    )}
                    {published.length > 0 && (
                      <div>
                        <b>Published:</b>{" "}
                        {published
                          .map((id) => gameNames[id] || `#${id}`)
                          .join(", ")}
                      </div>
                    )}
                  </div>
                )}
              </div>

              <div style={{ display: "flex", gap: 8 }}>
                <button
                  onClick={() => handleSelect(c)}
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
                  onClick={() => handleDelete(c.companyId)}
                  disabled={hasAnyGames}
                  title={
                    hasAnyGames
                      ? "Cannot delete company with associated games."
                      : ""
                  }
                  style={{
                    background: hasAnyGames ? "#9ca3af" : "#dc2626",
                    color: "white",
                    border: "none",
                    borderRadius: 6,
                    padding: "6px 10px",
                    cursor: hasAnyGames ? "not-allowed" : "pointer",
                    opacity: hasAnyGames ? 0.7 : 1,
                  }}
                >
                  Delete
                </button>
              </div>
            </div>
          );
        })}

        {companies.length === 0 && !loading && (
          <div style={{ color: "#555", padding: 10 }}>No companies found.</div>
        )}
      </div>
    </div>
  );
}
