import { useEffect, useMemo, useState } from "react";
import { GamesAPI } from "../api/games";
import { GenresAPI } from "../api/genres";


/* tiny pill for platforms/genres */

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

/* one row in the left list */
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
          {game.genres.slice(0, 2).map((id, i) => (
            <span key={id}>
              {i > 0 ? " · " : ""}
              {genreNameFor(id)}
            </span>
          ))}
          {game.genres.length > 2 ? " · …" : ""}
        </div>
        <div style={{ fontSize: 12, color: "#999" }}>
          {new Date(game.releaseDate).getFullYear()}
        </div>
      </div>
    </button>
  );
}

export default function GamesPage() {
  // raw data from backend
  const [allGames, setAllGames] = useState([]);
  const [genresById, setGenresById] = useState(new Map()); // id to name mapper

  // ui bits
  const [search, setSearch] = useState("");
  const [genre, setGenre] = useState("ALL"); // stores a genreId or "ALL"
  const [selected, setSelected] = useState(null);

  // request state
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // load games + genres together so we can render names immediately
  useEffect(() => {
    let cancel = false;
    (async () => {
      try {
        setLoading(true);
        setError("");
        const [gamesRaw, genresRaw] = await Promise.all([     //THIS IS WHERE WE CALL OUR APIS
          GamesAPI.getAll(),   // [{ gameId, name, releaseDate, publisherId, developerId, platforms, genres, imageUrl }]
          GenresAPI.getAll(),  // [{ genreId, name, games: [] }]
        ]);
        if (cancel) return;

        // build id maps to name map; normalize ids to strings for consistent lookup
        const map = new Map(
          (genresRaw || []).map((g) => [String(g.genreId), g.name]) //null safety before mapping
        );
        setGenresById(map);

        setAllGames(gamesRaw || []);
        setSelected((gamesRaw && gamesRaw[0]) || null); // preselect something
      } catch (e) {
        console.error(e);
        setError(e.message || "Failed to load games.");
      } finally {
        if (!cancel) setLoading(false);
      }
    })();
    return () => {
      cancel = true;
    };
  }, []);

  // helper turns a genreId into a readable label
  const genreNameFor = (id) => genresById.get(String(id)) || `Genre ${id}`;

  // dropdown options whatever actually exists in the list 
  const allGenres = useMemo(() => {
    const s = new Set();
    allGames.forEach((g) => (g.genres || []).forEach((id) => s.add(String(id))));
    const arr = Array.from(s).sort((a, b) =>
      genreNameFor(a).toLowerCase().localeCompare(genreNameFor(b).toLowerCase())
    );
    return ["ALL", ...arr];
  }, [allGames, genresById]);

  // filter left list by search text (name) + selected genre
  const filtered = useMemo(() => {
    const t = search.trim().toLowerCase();
    return allGames.filter((g) => {
      const byText = t ? g.name.toLowerCase().includes(t) : true;
      const byGenre =
        genre === "ALL" ? true : (g.genres || []).map(String).includes(genre);
      return byText && byGenre;
    });
  }, [allGames, search, genre]);

  const handleSelect = (game) => setSelected(game);

  return (
    <section className="page" style={{ maxWidth: 1200, margin: "0 auto" }}>
      <h2>Games</h2>

      {error && <div style={{ color: "#b00020", marginBottom: 12 }}>{error}</div>}
      {loading ? (
        <div>Loading…</div>
      ) : (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "360px 1fr", // left list / right details
            gap: 24,
            alignItems: "start",
          }}
        >
          {/* left search, filter, results */}
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
                placeholder="Search games…"
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
                  key={g.gameId}
                  game={g}
                  isSelected={selected?.gameId === g.gameId}
                  onClick={handleSelect}
                  genreNameFor={genreNameFor}
                />
              ))}
              {filtered.length === 0 && (
                <div style={{ color: "#777", fontSize: 14, padding: "12px 4px" }}>
                  No games match your filters.
                </div>
              )}
            </div>
          </aside>

          {/* right details for the selected game */}
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

                    {/* only show fields your backend actually returns */}
                    <dl style={{ margin: 0, fontSize: 14, color: "#333" }}>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Release Date</dt>
                        <dd>{new Date(selected.releaseDate).toLocaleDateString()}</dd>
                      </div>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Publisher ID</dt>
                        <dd>{selected.publisherId ?? "—"}</dd>
                      </div>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Developer ID</dt>
                        <dd>{selected.developerId ?? "—"}</dd>
                      </div>
                    </dl>

                    <div style={{ marginTop: 10 }}>
                      <div style={{ fontSize: 12, color: "#666", marginBottom: 4 }}>
                        Platforms
                      </div>
                      {(selected.platforms || []).map((p) => (
                        <Chip key={p} text={p} />
                      ))}
                    </div>

                    <div style={{ marginTop: 10 }}>
                      <div style={{ fontSize: 12, color: "#666", marginBottom: 4 }}>
                        Genres
                      </div>
                      {(selected.genres || []).map((id) => (
                        <Chip key={id} text={genreNameFor(id)} />
                      ))}
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