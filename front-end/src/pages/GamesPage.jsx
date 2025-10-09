import { useEffect, useMemo, useState } from "react";
import { GamesAPI } from "../api/games";
import { GenresAPI } from "../api/genres";
import { CompaniesAPI } from "../api/companies";
import { PlatformsAPI } from "../api/platforms";

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
          {game.genres.slice(0, 2).map((id, i) => (
            <span key={id}>
              {i > 0 ? ", " : ""}
              {genreNameFor(id)}
            </span>
          ))}
          {game.genres.length > 2 ? " ..." : ""}
        </div>
        <div style={{ fontSize: 12, color: "#999" }}>
          {new Date(game.releaseDate).getFullYear()}
        </div>
      </div>
    </button>
  );
}

export default function GamesPage() {
  // We store raw data from the backend.
  const [allGames, setAllGames] = useState([]);
  const [genresById, setGenresById] = useState(new Map()); // We map genre id to name.

  // We store lookup maps for companies and platforms.
  const [companiesById, setCompaniesById] = useState(new Map()); // We map company id to name.
  const [platformsById, setPlatformsById] = useState(new Map()); // We map platform id to name.

  // We store UI state for filters and selection.
  const [search, setSearch] = useState("");
  const [genre, setGenre] = useState("ALL"); // We store a genre id or "ALL".
  const [selected, setSelected] = useState(null);

  // We track request state.
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // We load games, genres, companies, and platforms together so we can render names immediately.
  useEffect(() => {
    let cancel = false;
    (async () => {
      try {
        setLoading(true);
        setError("");

        // We call all APIs in parallel.
        const [gamesRaw, genresRaw, companiesRaw, platformsRaw] = await Promise.all([
          GamesAPI.getAll(),
          GenresAPI.getAll(),
          CompaniesAPI.getAll(),
          PlatformsAPI.getAll(),
        ]);
        if (cancel) return;

        // We build a map from genre id (as string) to name.
        const genresMap = new Map(
          (genresRaw || []).map((g) => [String(g.genreId), g.name])
        );
        setGenresById(genresMap);

        // We build a map from company id (as string) to name.
        // We support either companyId or id based on the payload shape.
        const companiesMap = new Map(
          (companiesRaw || []).map((c) => [String(c.companyId ?? c.id), c.name])
        );
        setCompaniesById(companiesMap);

        // We build a map from platform id (as string) to name.
        const platformsMap = new Map(
          (platformsRaw || []).map((p) => [String(p.platformId), p.name])
        );
        setPlatformsById(platformsMap);

        // We store the raw game list and preselect the first game when available.
        setAllGames(gamesRaw || []);
        setSelected((gamesRaw && gamesRaw[0]) || null);
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

  // We resolve a genre id to a human-readable label.
  const genreNameFor = (id) => genresById.get(String(id)) || `Genre ${id}`;

  // We resolve a company id to a company name.
  const companyNameFor = (id) =>
    id == null ? "None" : (companiesById.get(String(id)) || `Company ${id}`);

  // We resolve a platform id to a platform name.
  const platformNameFor = (id) =>
    platformsById.get(String(id)) || `Platform ${id}`;

  // We compute available genres based on the current game list.
  const allGenres = useMemo(() => {
    const s = new Set();
    allGames.forEach((g) => (g.genres || []).forEach((id) => s.add(String(id))));
    const arr = Array.from(s).sort((a, b) =>
      genreNameFor(a).toLowerCase().localeCompare(genreNameFor(b).toLowerCase())
    );
    return ["ALL", ...arr];
  }, [allGames, genresById]);

  // We filter the left list by search text and selected genre.
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
        <div>Loading...</div>
      ) : (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "360px 1fr", // left list / right details
            gap: 24,
            alignItems: "start",
          }}
        >
          {/* We render the search, filter, and results list on the left. */}
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
                  key={g.gameId}
                  game={g}
                  isSelected={selected?.gameId === g.gameId}
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

          {/* We render details for the selected game on the right. */}
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

                    {/* We show mapped names for publisher and developer. */}
                    <dl style={{ margin: 0, fontSize: 14, color: "#333" }}>
                      <div style={{ display: "grid", gridTemplateColumns: "120px 1fr" }}>
                        <dt style={{ color: "#666" }}>Release Date</dt>
                        <dd>{new Date(selected.releaseDate).toLocaleDateString()}</dd>
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

                    <div style={{ marginTop: 10 }}>
                      <div style={{ fontSize: 12, color: "#666", marginBottom: 4 }}>
                        Platforms
                      </div>
                      {(selected.platforms || []).map((p) => (
                        <Chip key={p} text={platformNameFor(p)} />
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
