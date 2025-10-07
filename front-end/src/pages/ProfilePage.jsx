import { useEffect, useMemo, useState } from "react";
import { useApp } from "../context/AppContext";
import snake from "../assets/snake.jpg";
import { getMe, getUserGames, getUserReviews, getReview } from "../api/profile";

/* ---------------- STUB DATA replace later with real API ---------------- */
const STUB_ME = { id: 42, username: "test", avatarUrl: null, favoriteGenres: ["Action", "Indie"] };
const STUB_GAMES = [
  { gameId: 101, title: "Hollow Knight", coverUrl: "", rating: 5, lastPlayed: "2025-09-28", reviewId: 9001 },
  { gameId: 202, title: "Celeste",       coverUrl: "", rating: 4, lastPlayed: "2025-09-21", reviewId: null   },
  { gameId: 303, title: "Hades",         coverUrl: "", rating: 5, lastPlayed: "2025-09-15", reviewId: 9002  },
];
const STUB_REVIEWS = [
  { reviewId: 9001, gameId: 101, gameTitle: "Hollow Knight", rating: 5, body: "Movement is buttery and the worldbuilding sings.", createdAt: "2025-09-28T14:20:00Z" },
  { reviewId: 9002, gameId: 303, gameTitle: "Hades",         rating: 5, body: "Combat loop is addictive; great VO and pacing.",   createdAt: "2025-09-22T10:05:00Z" },
];
// simulate latency so you can see loading states
const sleep = (ms) => new Promise(r => setTimeout(r, ms));
async function getMeStub() { await sleep(250); return STUB_ME; }
async function getUserGamesStub(/*userId*/) { await sleep(250); return STUB_GAMES; }
async function getUserReviewsStub(/*userId*/) { await sleep(250); return STUB_REVIEWS; }
async function getReviewStub(id) { await sleep(150); return STUB_REVIEWS.find(r => r.reviewId === id) || null; }
/* ------------------------------------------------------------------------ */

const Avatar = ({ src, alt }) => (
  <img
    src={src || snake}  // fallback to snake logo otherwise we can set pfps
    alt={alt || "avatar"} //if neither img works
    style={{ width: 96, height: 96, borderRadius: "50%", objectFit: "cover", border: "1px solid #ddd" }}
  />
);
const Stars = ({ n = 0, outOf = 5 }) => <span>{"★".repeat(n)}{"☆".repeat(outOf - n)}</span>; //out of 5 stars
/** GameRow
 * Renders one row in the Played Games list
 * Props:
 *  - game: object with {title, coverUrl, rating, lastPlayed, reviewId}
 *  - isSelected: highlight this row if it's the one currently selected
 *  - onClick: callback fired with the game when the row is clicked
 */
function GameRow({ game, isSelected, onClick }) {
  return (
    <button
      onClick={() => onClick(game)}
      className={`row ${isSelected ? "row--selected" : ""}`}
      style={{ display: "flex", gap: 12, padding: 10, width: "100%", textAlign: "left", border: "1px solid #eee", borderRadius: 8, background: isSelected ? "#f6f8ff" : "#fff" }} //cover url we cna remove later if no need on line 47 not sure why my comment wasnt working
    > 
      <img src={game.coverUrl || "https://placehold.co/56"} alt="" width={56} height={56} style={{ borderRadius: 6, objectFit: "cover" }} /> 
      <div style={{ flex: 1 }}>
        <div style={{ fontWeight: 600 }}>{game.title}</div>
        <div style={{ fontSize: 12, color: "#666" }}>
          <Stars n={game.rating || 0} /> {game.lastPlayed ? `· Last played ${game.lastPlayed}` : ""}
        </div>
        <div style={{ fontSize: 12, color: game.reviewId ? "#2f6f2f" : "#999" }}>
          {game.reviewId ? "Has review" : "No review yet"}
        </div>
      </div>
    </button>
  );
}


/** ReviewRow
 * Renders one row in the Reviews list.
 * Props:
 *  - review: {gameTitle, rating, body, createdAt}
 *  - isSelected: highlight this row if selected
 *  - onClick: callback fired with the review when clicked
 */
function ReviewRow({ review, isSelected, onClick }) {
  return (
    <button
      onClick={() => onClick(review)}
      className={`row ${isSelected ? "row--selected" : ""}`}
      style={{ display: "block", padding: 10, width: "100%", textAlign: "left", border: "1px solid #eee", borderRadius: 8, background: isSelected ? "#f6f8ff" : "#fff" }} //inline style for now
    >
      <div style={{ fontWeight: 600 }}>{review.gameTitle}</div>
      <div style={{ fontSize: 12, color: "#666", margin: "2px 0 6px" }}>
        <Stars n={review.rating || 0} /> · {new Date(review.createdAt).toLocaleDateString()}
      </div>
      <div style={{ fontSize: 13, color: "#333" }}>
        {review.body.length > 120 ? review.body.slice(0, 120) + "…" : review.body}
      </div>
    </button>
  );
}

export default function ProfilePage() {
  const { username, userId, avatarUrl, setUsername, setUserId, setAvatarUrl } = useApp();
// global session values from context its like a bunch of fields shared on a singleton
  const [games, setGames] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [selected, setSelected] = useState(null); // { game, review|null }
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // initial load using stubs
  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        setLoading(true);
        const me = await getMeStub();
        if (cancelled) return;
        if (!userId) {
          setUserId(me.id);
          setUsername(me.username);
          setAvatarUrl(me.avatarUrl ?? null); //temporary but would be cool
        }
        const [g, r] = await Promise.all([getUserGamesStub(me.id), getUserReviewsStub(me.id)]);
        if (cancelled) return;
        setGames(g);
        setReviews(r);
      } catch (e) {
        setError("Failed to load profile.");
        console.error(e);
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => { cancelled = true; };
  }, [userId, setUserId, setUsername, setAvatarUrl]);

  const favoriteGenres = useMemo(() => STUB_ME.favoriteGenres.join(", "), []);

    /**
   * handleGameClick(game)
   * When a game is clicked
   * if it has a reviewId, we fetch/find that review and show it
   * otherwise we show the game header and output No review yet
   */
  const handleGameClick = async (game) => {
    if (game.reviewId) {
      const review =
        reviews.find(r => r.reviewId === game.reviewId) ||
        (await getReviewStub(game.reviewId));
      setSelected({ game, review });
    } else {
      setSelected({ game, review: null });
    }
  };
  /**
   * handleReviewClick(review)
   * when a review is clicked
   *  - locate the matching game (or construct a minimal one),
   *  - then show both in the detail panel.
   */
  const handleReviewClick = (review) => {
    const game = games.find(g => g.gameId === review.gameId) || { gameId: review.gameId, title: review.gameTitle, coverUrl: "" }; //either find the game or make a new one
    setSelected({ game, review });
  };

  return (
    <section className="page" style={{ maxWidth: 1100, margin: "0 auto" }}> 
      <h2>Profile</h2> 
       
      {/* header */} 
      <div style={{ display: "flex", gap: 16, alignItems: "center", marginBottom: 16 }}>
        <Avatar src={avatarUrl} alt={`${username} avatar`} /> 
        <div>
          <div>Logged in as <strong>{username}</strong>.</div>
          <div style={{ fontSize: 13, color: "#666" }}>Favorite Genres: {favoriteGenres}</div> 
        </div>
      </div>
                                                                                                          
      {error && <div style={{ color: "#b00020", marginBottom: 12 }}>{error}</div>}
      {loading ? <div>Loading…</div> : (
        <>
          {/* two columns */}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 24, alignItems: "start" }}>
            <section>
              <h3>Played Games</h3>
              <div style={{ display: "grid", gap: 8 }}>
                {games.map(g => (
                  <GameRow key={g.gameId} game={g} isSelected={selected?.game?.gameId === g.gameId} onClick={handleGameClick} />
                ))}
              </div>
            </section>

            <section>
              <h3>Reviews</h3>
              <div style={{ display: "grid", gap: 8 }}>
                {reviews.map(r => (
                  <ReviewRow key={r.reviewId} review={r} isSelected={selected?.review?.reviewId === r.reviewId} onClick={handleReviewClick} />
                ))}
              </div>
            </section>
          </div>

          {/* detail panel */}
          <section style={{ marginTop: 24 }}>
            {selected ? (
              <div style={{ border: "1px solid #eee", borderRadius: 12, padding: 16, background: "#fafafa" }}>
                <div style={{ display: "flex", gap: 12, alignItems: "center", marginBottom: 8 }}>
                  <img src={selected.game.coverUrl || "https://placehold.co/72"} alt="" width={72} height={72} style={{ borderRadius: 8, objectFit: "cover" }} />
                  <div>
                    <h4 style={{ margin: 0 }}>{selected.game.title}</h4>
                    {selected.review ? <Stars n={selected.review.rating} /> : <span style={{ color: "#999", fontSize: 13 }}>No review yet</span>}
                  </div>
                </div>
                {selected.review && <p style={{ marginTop: 8, whiteSpace: "pre-wrap" }}>{selected.review.body}</p>}
              </div>
            ) : (
              <div style={{ color: "#777", fontSize: 14 }}>Select a game or review to view details here.</div>
            )}
          </section>
        </>
      )}
    </section>
  );
}
