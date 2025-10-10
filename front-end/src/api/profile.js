import { apiFetch } from "./client";

// Resolve a user by username getting id
export function getUserByUsername(username) {
  return apiFetch(`/User/username/${encodeURIComponent(username)}`);
}

// includes gameLibrary
export function getUserById(userId) {
  return apiFetch(`/User/${userId}`);
}

export function getGameById(id) {
  return apiFetch(`/Games/${id}`);
}

// get many games by id 
export async function getGamesForIds(ids) {
  const unique = Array.from(new Set((ids || []).filter(Boolean)));
  const results = await Promise.all(
    unique.map(id => getGameById(id).catch(() => null))
  );
  return results.filter(Boolean);
}
