import { useEffect, useState, useMemo } from "react";
import { useApp } from "../context/AppContext";
import snake from "../assets/snake.jpg";
import { AuthAPI } from "../api/auth";
import { getUserByUsername, getUserById, getGamesForIds } from "../api/profile";
import * as RatingsAPI from "../api/ratings";

const Avatar = ({ src, alt }) => (
  <img
    src={src || snake}
    alt={alt || "avatar"}
    style={{ width: 96, height: 96, borderRadius: "50%", objectFit: "cover", border: "1px solid #ddd" }}
  />
);

const Stars = ({ n = 0, outOf = 5 }) => <span>{"★".repeat(n)}{"☆".repeat(Math.max(0, outOf - n))}</span>;

function StarPicker({ value, onChange }) {
  return (
    <div role="radiogroup" aria-label="Rating" style={{ display: "flex", gap: 6 }}>
      {[1,2,3,4,5].map(k => (
        <button
          key={k}
          type="button"
          onClick={() => onChange(k)}
          aria-checked={value === k}
          role="radio"
          style={{
            fontSize: 20,
            lineHeight: "20px",
            padding: "2px 4px",
            borderRadius: 6,
            border: "1px solid #ddd",
            background: value >= k ? "#fffbe6" : "#fff",
            cursor: "pointer",
          }}
          title={`${k} star${k>1?"s":""}`}
        >
          {k <= value ? "★" : "☆"}
        </button>
      ))}
    </div>
  );
}

function GameRow({ game, isSelected, onClick }) {
  return (
    <button
      onClick={() => onClick(game)}
      className={`row ${isSelected ? "row--selected" : ""}`}
      style={{ display: "flex", gap: 12, padding: 10, width: "100%", textAlign: "left", border: "1px solid #eee", borderRadius: 8, background: isSelected ? "#f6f8ff" : "#fff" }}
    >
      <img src={game.imageUrl || game.coverUrl || "https://placehold.co/56"} alt="" width={56} height={56} style={{ borderRadius: 6, objectFit: "cover" }} />
      <div style={{ flex: 1 }}>
        <div style={{ fontWeight: 600 }}>{game.name ?? game.title ?? "Untitled"}</div>
        <div style={{ fontSize: 12, color: "#666" }}>
          <Stars n={game._myRate || 0} />
        </div>
      </div>
    </button>
  );
}

function ReviewRow({ review, isSelected, onClick }) {
  return (
    <button
      onClick={() => onClick(review)}
      className={`row ${isSelected ? "row--selected" : ""}`}
      style={{ display: "block", padding: 10, width: "100%", textAlign: "left", border: "1px solid #eee", borderRadius: 8, background: isSelected ? "#f6f8ff" : "#fff" }}
    >
      <div style={{ fontWeight: 600 }}>{review.gameTitle}</div>
      <div style={{ fontSize: 12, color: "#666", margin: "2px 0 6px" }}>
        <Stars n={review.rate || 0} /> · {review.dateTimeRated ? new Date(review.dateTimeRated).toLocaleDateString() : "—"}
      </div>
      <div style={{ fontSize: 13, color: "#333" }}>
        {review.title ? <strong>{review.title}</strong> : "—"}
        {review.description ? ` — ${review.description.length > 120 ? review.description.slice(0, 120) + "…" : review.description}` : ""}
      </div>
    </button>
  );
}

export default function ProfilePage() {
  const { username, userId, avatarUrl, setUsername, setUserId, setAvatarUrl } = useApp();

  const [games, setGames] = useState([]);          // library games
  const [ratings, setRatings] = useState([]);      // my ratings
  const [selected, setSelected] = useState(null);  // selected game
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [authorized, setAuthorized] = useState(false);

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [rate, setRate] = useState(0);
  const [saving, setSaving] = useState(false);
  const [toast, setToast] = useState("");

  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        setLoading(true);
        setError("");
        setToast("");

        await AuthAPI.testAuthorization().catch(() => { throw new Error("Please log in to view your profile."); });
        if (cancelled) return;
        setAuthorized(true);

        const uname = typeof username === "string" ? username : (username?.username || "");
        if (!uname) { setError("No username in session. Log in again."); return; }

        // resolve id to username
        const meByName = await getUserByUsername(uname);
        if (cancelled) return;
        const id = meByName?.userId ?? meByName?.id;
        if (!id) { setError("Could not resolve user id."); return; }
        if (!userId) setUserId(id);
        if (!avatarUrl) setAvatarUrl(meByName?.avatarUrl ?? null);
        if (!username || typeof username !== "string") setUsername(meByName?.username ?? uname);

        // fetch full user to library ids
        const me = await getUserById(id);
        if (cancelled) return;
        const libraryIds = me?.gameLibrary || [];

        // fetch games
        const gameObjs = await getGamesForIds(libraryIds);
        if (cancelled) return;

        // fetch my ratings
        const myRatings = await RatingsAPI.getByUser(id).catch(() => []);
        if (cancelled) return;

        // attach my rate to each game for quick display
        const byGameId = new Map(myRatings.map(r => [r.gameId, r]));
        const gamesWithRate = (gameObjs || []).map(g => {
          const gid = g.gameId ?? g.id;
          return { ...g, _myRate: byGameId.get(gid)?.rate ?? 0 };
        });

        setGames(gamesWithRate);
        setRatings(myRatings);

        const first = gamesWithRate[0] || null;
        setSelected(first);
        if (first) {
          const r = byGameId.get(first.gameId ?? first.id);
          setTitle(r?.title ?? "");
          setDescription(r?.description ?? "");
          setRate(r?.rate ?? 0);
        }
      } catch (e) {
        if (cancelled) return;
        console.error(e);
        setError(e?.message || "Failed to load profile.");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => { cancelled = true; };
  }, [username]);

  // when user clicks game, prefill editor from rating
  const handleSelectGame = (game) => {
    setSelected(game);
    const gid = game?.gameId ?? game?.id;
    const r = ratings.find(x => x.gameId === gid);
    setTitle(r?.title ?? "");
    setDescription(r?.description ?? "");
    setRate(r?.rate ?? 0);
  };

  // Build a list of review objects we can actually display.
  // take each rating and add in the matching game name from the games list.
  const uiReviews = useMemo(() => {
    const nameById = new Map(games.map(g => [g.gameId ?? g.id, g.name ?? g.title ?? `Game #${g.gameId ?? g.id}`]));
    return ratings.map(r => ({
      id: r.id ?? `${r.userId}-${r.gameId}`,
      gameId: r.gameId,
      gameTitle: nameById.get(r.gameId) ?? `Game #${r.gameId}`,
      title: r.title ?? "",
      description: r.description ?? "",
      rate: r.rate ?? 0,
      dateTimeRated: r.dateTimeRated ?? null,
    }));
  }, [ratings, games]);

  async function handleSaveReview() {
    if (!userId || !selected) return;
    const gid = selected.gameId ?? selected.id;
    if (!gid) return;

    //  rate 1-5 else fail
    if (!title.trim()) {
      setToast("Please enter a title for your review.");
      return;
    }
    if (!rate || rate < 1 || rate > 5) {
      setToast("Please choose a star rating (1–5).");
      return;
    }

    const dto = {
      title: title.trim(),
      description: description.trim(),
      rate,
      dateTimeRated: new Date().toISOString(),
    };

    try {
      setSaving(true);
      setToast("");

      const existing = ratings.find(r => r.gameId === gid);
      if (existing) {
        await RatingsAPI.update(userId, gid, dto);
      } else {
        await RatingsAPI.create({ userId, gameId: gid, ...dto });
      }

      // update local ratings
      let newRatings;
      if (existing) {
        newRatings = ratings.map(r => r.gameId === gid ? { ...r, ...dto, gameId: gid, userId } : r);
      } else {
        newRatings = [...ratings, { userId, gameId: gid, ...dto }];
      }
      setRatings(newRatings);

      // reflect rate on game rows
      setGames(prev =>
        prev.map(g => ((g.gameId ?? g.id) === gid ? { ...g, _myRate: rate } : g))
      );

      setToast("Review saved!");
      setTimeout(() => setToast(""), 2000);
    } catch (e) {
      console.error(e);
      setToast(e?.message || "Failed to save review.");
    } finally {
      setSaving(false);
    }
  }

  const handleReviewClick = (review) => {
    const game = games.find(g => (g.gameId ?? g.id) === review.gameId);
    if (game) {
      setSelected(game);
      setTitle(review.title ?? "");
      setDescription(review.description ?? "");
      setRate(review.rate ?? 0);
    }
  };

  return (
    <section className="page" style={{ maxWidth: 1100, margin: "0 auto" }}>
      <h2>Profile</h2>

      {/* header */}
      <div style={{ display: "flex", gap: 16, alignItems: "center", marginBottom: 16 }}>
        <Avatar src={avatarUrl} alt={`${String(username || "")} avatar`} />
        <div>
          <div>Logged in as <strong>{String(username || "—")}</strong>.</div>
        </div>
      </div>

      {toast && <div style={{ color: "#2f6f2f", marginBottom: 8 }}>{toast}</div>}
      {error && <div style={{ color: "#b00020", marginBottom: 12 }}>{error}</div>}

      {loading ? (
        <div>Loading…</div>
      ) : !authorized ? (
        <div style={{ color: "#555", fontSize: 14 }}>We are not authorized to view this page. Please log in.</div>
      ) : (
        <>
          {/* two columns */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 24, alignItems: "start" }}>
            {/* My Library */}
            <section>
              <h3>My Library</h3>
              <div style={{ display: "grid", gap: 8 }}>
                {(games || []).map(g => (
                  <GameRow
                    key={g.gameId ?? g.id}
                    game={g}
                    isSelected={(selected?.gameId ?? selected?.id) === (g.gameId ?? g.id)}
                    onClick={handleSelectGame}
                  />
                ))}
                {(!games || games.length === 0) && (
                  <div style={{ color: "#777", fontSize: 14 }}>No games in your library yet.</div>
                )}
              </div>
            </section>

            {/* My Reviews */}
            <section>
              <h3>My Reviews</h3>
              <div style={{ display: "grid", gap: 8 }}>
                {(uiReviews || []).map(r => (
                  <ReviewRow
                    key={r.id}
                    review={r}
                    isSelected={selected && (selected.gameId ?? selected.id) === r.gameId}
                    onClick={handleReviewClick}
                  />
                ))}
                {(!uiReviews || uiReviews.length === 0) && (
                  <div style={{ color: "#777", fontSize: 14 }}>No reviews yet.</div>
                )}
              </div>
            </section>
          </div>

          {/* detail + review editor */}
          <section style={{ marginTop: 24 }}>
            {selected ? (
              <div style={{ border: "1px solid #eee", borderRadius: 12, padding: 16, background: "#fafafa" }}>
                <div style={{ display: "flex", gap: 12, alignItems: "center", marginBottom: 8 }}>
                  <img
                    src={selected.imageUrl || selected.coverUrl || "https://placehold.co/72"}
                    alt=""
                    width={72}
                    height={72}
                    style={{ borderRadius: 8, objectFit: "cover" }}
                  />
                  <div>
                    <h4 style={{ margin: 0 }}>{selected.name ?? selected.title ?? "Untitled"}</h4>
                    <div style={{ fontSize: 12, color: "#666" }}>
                      Your current rating: <Stars n={rate || 0} />
                    </div>
                  </div>
                </div>

                {/* Review Editor*/}
                <div style={{ display: "grid", gap: 10, marginTop: 10 }}>
                  <label style={{ fontSize: 13, color: "#555" }}>Title (required)</label>
                  <input
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder="e.g., ‘Amazing gameplay loop’"
                    style={{ width: "100%", border: "1px solid #ddd", borderRadius: 8, padding: 10 }}
                  />

                  <label style={{ fontSize: 13, color: "#555", marginTop: 8 }}>Your rating</label>
                  <StarPicker value={rate} onChange={setRate} />

                  <label style={{ fontSize: 13, color: "#555", marginTop: 8 }}>Description (optional)</label>
                  <textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="What did you think?"
                    rows={5}
                    style={{ width: "100%", border: "1px solid #ddd", borderRadius: 8, padding: 10, resize: "vertical" }}
                  />

                  <div>
                    <button
                      onClick={handleSaveReview}
                      disabled={saving || !title.trim() || !rate}
                      style={{
                        padding: "8px 12px",
                        borderRadius: 8,
                        border: "1px solid #d1d5db",
                        background: saving ? "#e5e7eb" : "#fff",
                        cursor: saving ? "not-allowed" : "pointer",
                      }}
                    >
                      {saving ? "Saving…" : "Save Review"}
                    </button>
                    <span style={{ marginLeft: 10, fontSize: 12, color: "#666" }}>
                      {(!title.trim() || !rate) ? "Enter a title and pick 1–5 stars." : ""}
                    </span>
                  </div>
                </div>
              </div>
            ) : (
              <div style={{ color: "#777", fontSize: 14 }}>
                Select a game to write a review.
              </div>
            )}
          </section>
        </>
      )}
    </section>
  );
}
