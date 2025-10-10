import { useEffect, useMemo, useState } from "react";
import { useApp } from "../context/AppContext";
import { AuthAPI } from "../api/auth";
import { GamesAPI } from "../api/games";
import { GenresAPI } from "../api/genres";
import { CompaniesAPI } from "../api/companies";
import { PlatformsAPI } from "../api/platforms";
import { UsersAPI } from "../api/users";

/* We render a small chip element for platform and genre labels. */
const Chip = ({ text }) => (
  <span
    style={{
      display: "inline-block",
      fontSize: 12,
      padding: "2px 8px",
      borderRadius: 12,
      border: "1px solid #e2e8f0",
      background: "#f8fafc",
      marginRight: 6,
      marginBottom: 6,
    }}
  >
    {text}
  </span>
);

/* We render one selectable game row in the left list. */
function GameListItem({ game, isSelected, onClick, genreNameFor }) {
  return (
    <button
      onClick={() => onClick(game)}
      className={`row ${isSelected ? "row--selected" : ""}`}
      style={{
        display: "flex",
        gap: 12,
        padding: 10,
        width: "100%",
        textAlign: "left",
        border: "1px solid #eee",
        borderRadius: 8,
        background: isSelected ? "#f6f8ff" : "#fff",
      }}
    >
      <img
        src={game.imageUrl || "https://placehold.co/56"}
        alt=""
        width={56}
        height={56}
        style={{ borderRadius: 6, objectFit: "cover" }}
      />
      <div style={{ flex: 1 }}>
        <div style={{ fontWeight: 600 }}>{game.name}</div>
        <div style={{ fontSize: 12, color: "#666" }}>
          {(game.genres || []).slice(0, 2).map((id, i) => (
            <span key={id}>
              {i > 0 ? ", " : ""}
              {genreNameFor(id)}
            </span>
          ))}
          {(game.genres || []).length > 2 ? " ..." : ""}
        </div>
        <div style={{ fontSize: 12, color: "#999" }}>
          {game.releaseDate ? new Date(game.releaseDate).getFullYear() : "—"}
        </div>
      </div>
    </button>
  );
}

export default function GamesPage() {
  const { userId } = useApp(); 

  // We store raw data from the backend.
  const [allGames, setAllGames] = useState([]);
  const [genresById, setGenresById] = useState(new Map());       // genre id to name
  const [companiesById, setCompaniesById] = useState(new Map()); // company id to name
  const [platformsById, setPlatformsById] = useState(new Map()); // platform id to name

  // UI state
  const [search, setSearch] = useState("");
  const [genre, setGenre] = useState("ALL");
  const [selected, setSelected] = useState(null);

  // Request/auth state
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [authorized, setAuthorized] = useState(false);

  // Add to library UI
  const [addingId, setAddingId] = useState(null);
  const [toast, setToast] = useState("");

  // Load after auth
  useEffect(() => {
    let cancel = false;

    (async () => {
      try {
        setLoading(true);
        setError("");
        setToast("");

        await AuthAPI.testAuthorization();
        if (cancel) return;
        setAuthorized(true);

        const [gamesRaw, genresRaw, companiesRaw, platformsRaw] = await Promise.all([
          GamesAPI.getAll(),
          GenresAPI.getAll(),
          CompaniesAPI.getAll(),
          PlatformsAPI.getAll(),
        ]);
        if (cancel) return;

        const genresMap = new Map((genresRaw || []).map((g) => [String(g.genreId ?? g.id), g.name]));
        setGenresById(genresMap);

        const companiesMap = new Map((companiesRaw || []).map((c) => [String(c.companyId ?? c.id), c.name]));
        setCompaniesById(companiesMap);

        const platformsMap = new Map((platformsRaw || []).map((p) => [String(p.platformId ?? p.id), p.name]));
        setPlatformsById(platformsMap);

        const list = gamesRaw || [];
        setAllGames(list);
        setSelected(list[0] || null);
      } catch (e) {
        if (cancel) return;
        console.error(e);
        if (String(e.message || "").toLowerCase().includes("unauthorized")) {
          setError("We must be logged in to view games. Please log in and try again.");
          setAuthorized(false);
        } else {
          setError(e.message || "Failed to load games.");
        }
      } finally {
        if (!cancel) setLoading(false);
      }
    })();

    return () => { cancel = true; };
  }, []);

  // Helpers to resolve ids to labels
  const genreNameFor = (id) => genresById.get(String(id)) || `Genre ${id}`;
  const companyNameFor = (id) => (id == null ? "None" : (companiesById.get(String(id)) || `Company ${id}`));
  const platformNameFor = (id) => platformsById.get(String(id)) || `Platform ${id}`;

  // Available genres from games
  const allGenres = useMemo(() => {
    const s = new Set();
    allGames.forEach((g) => (g.genres || []).forEach((id) => s.add(String(id))));
    const arr = Array.from(s).sort((a, b) =>
      genreNameFor(a).toLowerCase().localeCompare(genreNameFor(b).toLowerCase())
    );
    return ["ALL", ...arr];
  }, [allGames, genresById]);

  // Filtered list
  const filtered = useMemo(() => {
    const t = search.trim().toLowerCase();
    return (allGames || []).filter((g) => {
      const byText = t ? (g.name || "").toLowerCase().includes(t) : true;
      const byGenre =
        genre === "ALL" ? true : (g.genres || []).map(String).includes(genre);
      return byText && byGenre;
    });
  }, [allGames, search, genre]);

  const handleSelect = (game) => setSelected(game);

  // Add to library
  async function handleAddToLibrary(game) {
    if (!userId) {
      setToast("Please log in first.");
      return;
    }
    const gid = game.gameId ?? game.id;
    try {
      setAddingId(gid);
      setToast("");
      await UsersAPI.linkGame(userId, gid);
      setToast(`Added "${game.name}" to your library.`);
    } catch (e) {
      console.error(e);
      setToast(e?.message || "Failed to add to library.");
    } finally {
      setAddingId(null);
      setTimeout(() => setToast(""), 2500);
    }
  }

  return (
    <section className="page" style={{ maxWidth: 1200, margin: "0 auto" }}>
      <h2>Games</h2>

      {toast && <div style={{ color: "#2f6f2f", marginBottom: 8 }}>{toast}</div>}
      {error && <div style={{ color: "#b00020", marginBottom: 12 }}>{error}</div>}

      {loading ? (
        <div>Loading...</div>
      ) : !authorized ? (
        <div style={{ color: "#555", fontSize: 14 }}>
          We are not authorized to view this page. Please log in.
        </div>
      ) : (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "360px 1fr", // left list / right details
            gap: 24,
            alignItems: "start",
          }}
        >
          {/* Left: search/filter + list */}
          <aside>
            <div
              style={{
                display: "flex",
                gap: 8,
                marginBottom: 10,
                alignItems: "center",
              }}
            >
              <input
                type="text"
                placeholder="Search games..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                style={{
                  flex: 1,
                  padding: "8px 10px",
                  borderRadius: 8,
                  border: "1px solid #e5e7eb",
                }}
              />
              <select
                value={genre}
                onChange={(e) => setGenre(e.target.value)}
                style={{
                  padding: "8px 10px",
                  borderRadius: 8,
                  border: "1px solid #e5e7eb",
                  background: "#fff",
                }}
                aria-label="Filter by genre"
              >
                {allGenres.map((id) =>
                  id === "ALL" ? (
                    <option key="ALL" value="ALL">All genres</option>
                  ) : (
                    <option key={id} value={id}>{genreNameFor(id)}</option>
                  )
                )}
              </select>
            </div>

            <div style={{ display: "grid", gap: 8 }}>
              {filtered.map((g) => (
                <GameListItem
                  key={g.gameId ?? g.id}
                  game={g}
                  isSelected={(selected?.gameId ?? selected?.id) === (g.gameId ?? g.id)}
                  onClick={handleSelect}
                  genreNameFor={genreNameFor}
                />
              ))}
              {filtered.length === 0 && (
                <div style={{ color: "#777", fontSize: 14, padding: "12px 4px" }}>
                  No games match the current filters.
                </div>
              )}
            </div>
          </aside>

          {/* Right: details + Add button */}
          <main>
            {selected ? (
              <div
                style={{
                  border: "1px solid #eee",
                  borderRadius: 12,
                  padding: 16,
                  background: "#fafafa",
                }}
              >
                <div
                  style={{
                    display: "grid",
                    gridTemplateColumns: "120px 1fr",
                    gap: 16,
                    alignItems: "start",
                    marginBottom: 8,
                  }}
                >
                  <img
                    src={selected.imageUrl || "https://placehold.co/120"}
                    alt=""
                    width={120}
                    height={120}
                    style={{ borderRadius: 8, objectFit: "cover" }}
                  />
                  <div>
                    <h3 style={{ margin: "0 0 6px" }}>{selected.name}</h3>

                    {/* Publisher/Developer */}
                    <dl style={{ margin: 0, fontSize: 14, color: "#333" }}>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Release Date</dt>
                        <dd>{selected.releaseDate ? new Date(selected.releaseDate).toLocaleDateString() : "—"}</dd>
                      </div>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Publisher</dt>
                        <dd>{companyNameFor(selected.publisherId)}</dd>
                      </div>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Developer</dt>
                        <dd>{companyNameFor(selected.developerId)}</dd>
                      </div>
                    </dl>

                    {/* Platforms */}
                    <div style={{ marginTop: 10 }}>
                      <div style={{ fontSize: 12, color: "#666", marginBottom: 4 }}>Platforms</div>
                      {(selected.platforms || []).map((p) => (
                        <Chip key={p} text={platformNameFor(p)} />
                      ))}
                    </div>

                    {/* Genres */}
                    <div style={{ marginTop: 10 }}>
                      <div style={{ fontSize: 12, color: "#666", marginBottom: 4 }}>Genres</div>
                      {(selected.genres || []).map((id) => (
                        <Chip key={id} text={genreNameFor(id)} />
                      ))}
                    </div>

                    {/* Add to My Library button */}
                    <div style={{ marginTop: 16 }}>
                      <button
                        onClick={() => handleAddToLibrary(selected)}
                        disabled={addingId === (selected.gameId ?? selected.id)}
                        style={{
                          padding: "8px 12px",
                          borderRadius: 8,
                          border: "1px solid #d1d5db",
                          background: addingId === (selected.gameId ?? selected.id) ? "#e5e7eb" : "#fff",
                          cursor: addingId === (selected.gameId ?? selected.id) ? "not-allowed" : "pointer",
                        }}
                      >
                        {addingId === (selected.gameId ?? selected.id) ? "Adding..." : "Add to My Library"}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ) : (
              <div style={{ color: "#777", fontSize: 14 }}>
                Select a game from the list to view details.
              </div>
            )}
          </main>
        </div>
      )}
    </section>
  );
}
