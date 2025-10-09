import { useEffect, useMemo, useState } from "react";
import { GamesAPI } from "../api/games";

/* ----------------------------------------------------------------------------
   STUB that mirrors our Swagger for GET /api/Games
   {
     gameId: number,
     name: string,
     releaseDate: ISO string,
     publisherId: number,
     developerId: number,
     platforms: string[],   // ids or codes from  backend
     genres: string[],      // ids  from  backend
     imageUrl: string
   }
---------------------------------------------------------------------------- */
const STUB_CATALOG = [
  {
    gameId: 1,
    name: "Hollow Knight",
    releaseDate: "2017-02-24T00:00:00Z",
    publisherId: 3,
    developerId: 3,
    platforms: ["pc", "switch", "ps4", "xbox"],
    genres: ["1", "2"], // pretend "1"=Action for example
    imageUrl: "https://placehold.co/256x256?text=Hollow+Knight",
  },
  {
    gameId: 2,
    name: "Celeste",
    releaseDate: "2018-01-25T00:00:00Z",
    publisherId: 5,
    developerId: 5,
    platforms: ["pc", "switch", "ps4", "xbox"],
    genres: ["2", "3"],
    imageUrl: "https://placehold.co/256x256?text=Celeste",
  },
  {
    gameId: 3,
    name: "Hades",
    releaseDate: "2020-09-17T00:00:00Z",
    publisherId: 7,
    developerId: 7,
    platforms: ["pc", "switch", "ps5", "xbox"],
    genres: ["1", "4"],
    imageUrl: "https://placehold.co/256x256?text=Hades",
  },
  {
    gameId: 4,
    name: "Stardew Valley",
    releaseDate: "2016-02-26T00:00:00Z",
    publisherId: 9,
    developerId: 9,
    platforms: ["pc", "switch", "ps4", "xbox", "mobile"],
    genres: ["5", "2"],
    imageUrl: "https://placehold.co/256x256?text=Stardew+Valley",
  },
];

/* NOTE on genre names
   backend returns genre IDs. We show IDs for now.
   when we want readable names before wiring /api/Genres, will need to add a tiny lookup
   for now we’ll keep it literally what the API returns. */

/* fake network delay so Loading… shows up */
// const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
// async function fetchGamesStub() {
//   await sleep(250);
//   return STUB_CATALOG;
// }

/* little mini component for platforms/genres */
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
function GameListItem({ game, isSelected, onClick }) {
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
        {/* show up to two genre IDs to keep the row tight */}
        <div style={{ fontSize: 12, color: "#666" }}>
          {game.genres.slice(0, 2).join(" · ")}
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
  // whole list from backend
  const [allGames, setAllGames] = useState([]);
  // ui controls
  const [search, setSearch] = useState("");
  const [genre, setGenre] = useState("ALL"); // dropdown value
  // selection + request state
  const [selected, setSelected] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");



  // first load stubs for now replace
  useEffect(() => {
    let cancel = false;
    (async () => {
      try {
        setLoading(true);
        setError("");
        const data = await GamesAPI.getAll();
        if (cancel) return;
        setAllGames(data);
        setSelected(data[0] ?? null); // preselect something so the right side isn’t empty
      } catch (e) {
        console.error(e);
        setError("Failed to load games.");
      } finally {
        if (!cancel) setLoading(false);
      }
    })();
    return () => {
      cancel = true;
    };
  }, []);

  // build the genre dropdown from what we actually have in the list IDs for now
  const allGenres = useMemo(() => {
    const s = new Set();
    allGames.forEach((g) => g.genres.forEach((id) => s.add(id)));
    return ["ALL", ...Array.from(s).sort()];
  }, [allGames]);

  // filter left list by search text (name) + selected genre id
  const filtered = useMemo(() => {
    const t = search.trim().toLowerCase();
    return allGames.filter((g) => {
      const byText = t ? g.name.toLowerCase().includes(t) : true;
      const byGenre = genre === "ALL" ? true : g.genres.includes(genre);
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
            gridTemplateColumns: "360px 1fr", // left list / right detail
            gap: 24,
            alignItems: "start",
          }}
        >
          {/* left panel with search + filter + results */}
          <aside>
            <div
              style={{
                display: "flex",
                gap: 8,
                marginBottom: 10,
                alignItems: "center",
              }}
            >
              {/* simple text search on game.name */}
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
              {/* genre dropdown uses IDs for now since thats what apis give us */}
              <select
                value={genre}
                onChange={(e) => setGenre(e.target.value)}
                style={{
                  padding: "8px 10px",
                  borderRadius: 8,
                  border: "1px solid #e5e7eb",
                  background: "#fff",
                }}
                aria-label="Filter by genre id"
              >
                {allGenres.map((g) => (
                  <option key={g} value={g}>
                    {g === "ALL" ? "All genres" : `Genre ${g}`}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ display: "grid", gap: 8 }}>
              {filtered.map((g) => (
                <GameListItem
                  key={g.gameId}
                  game={g}
                  isSelected={selected?.gameId === g.gameId}
                  onClick={handleSelect}
                />
              ))}
              {filtered.length === 0 && (
                <div style={{ color: "#777", fontSize: 14, padding: "12px 4px" }}>
                  No games match your filters.
                </div>
              )}
            </div>
          </aside>

          {/* right panel for details about whichever game is selected */}
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

                    {/* should match our backend */}
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
                      {selected.platforms.map((p) => (
                        <Chip key={p} text={p} />
                      ))}
                    </div>

                    <div style={{ marginTop: 10 }}>
                      <div style={{ fontSize: 12, color: "#666", marginBottom: 4 }}>
                        Genres (IDs)
                      </div>
                      {selected.genres.map((g) => (
                        <Chip key={g} text={g} />
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
