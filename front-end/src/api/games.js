import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/games");
}

function getById(gameId) {
  return apiFetch(`/games/${gameId}`);
}

// TODO: Waiting on backend implementation
// function getByGenre(genre) {
//   return apiFetch(`/games?genre=${encodeURIComponent(genre)}`);
// }

function getByName(name) {
  return apiFetch(`/games?name=${encodeURIComponent(name)}`);
}

function create(gameData) {
  return apiFetch("/games", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(gameData),
  });
}

function update(gameId, gameData) {
  return apiFetch(`/games/${gameId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(gameData),
  });
}

function remove(gameId) {
  return apiFetch(`/games/${gameId}`, {
    method: "DELETE",
  });
}

export const GamesAPI = {
  getAll,
  getById,
  getByName,
//   getByGenre,
  create,
  update,
  remove,
};
