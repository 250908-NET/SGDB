//const base = "/api"; // this is a temporary api file that we can edit later. For now its mostly stubs assuming we use jwt

/* This is for when our endpoints are online and we want to test them finalized
export async function getMe(token) {
  return fetch(`${base}/me`, { headers: { Authorization: `Bearer ${token}` }}).then(r => r.json()); //not sure what our endpoint for this is yet but for now i set /api/me
}
export async function getUserGames(userId, token) {
  return fetch(`${base}/users/${userId}/games`, { headers: { Authorization: `Bearer ${token}` }}).then(r => r.json());
}
export async function getUserReviews(userId, token) {
  return fetch(`${base}/users/${userId}/reviews`, { headers: { Authorization: `Bearer ${token}` }}).then(r => r.json());
}
export async function getReview(reviewId, token) {
  return fetch(`${base}/reviews/${reviewId}`, { headers: { Authorization: `Bearer ${token}` }}).then(r => r.json());
}
  */

// stub endpoints
const delay = (ms) => new Promise(r => setTimeout(r, ms));

export const STUB_ME = {
  id: 42,
  username: "test",
  avatarUrl: null,
  favoriteGenres: ["Action", "Indie"],
};

export const STUB_GAMES = [
  { gameId: 101, title: "Hollow Knight", coverUrl: "", rating: 5, lastPlayed: "2025-09-28", reviewId: 9001 },
  { gameId: 202, title: "Celeste",       coverUrl: "", rating: 4, lastPlayed: "2025-09-21", reviewId: null   },
  { gameId: 303, title: "Hades",         coverUrl: "", rating: 5, lastPlayed: "2025-09-15", reviewId: 9002  },
];

export const STUB_REVIEWS = [
  { reviewId: 9001, gameId: 101, gameTitle: "Hollow Knight", rating: 5, body: "Movement is buttery and the worldbuilding sings.", createdAt: "2025-09-28T14:20:00Z" },
  { reviewId: 9002, gameId: 303, gameTitle: "Hades",         rating: 5, body: "Combat loop is addictive; great VO and pacing.",   createdAt: "2025-09-22T10:05:00Z" },
];

// match the signatures our page already uses
export async function getMe(/* token */) {
  await delay(200); return STUB_ME;
}
export async function getUserGames(/* userId, token */) {
  await delay(200); return STUB_GAMES;
}
export async function getUserReviews(/* userId, token */) {
  await delay(200); return STUB_REVIEWS;
}
export async function getReview(reviewId /*, token */) {
  await delay(120); return STUB_REVIEWS.find(r => r.reviewId === reviewId) || null;
}

