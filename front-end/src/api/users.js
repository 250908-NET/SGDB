import { apiFetch } from "./client";


function getAll() {
  return apiFetch("/User");
}

function getById(userId) {
  return apiFetch(`/User/${userId}`);
}

function getByUsername(username) {
  return apiFetch(`/User/username/${encodeURIComponent(username)}`);
}

function create(userData) {
  return apiFetch("/User", {
    method: "POST",
    body: userData,
  });
}

function update(userId, userData) {
  return apiFetch(`/User/${userId}`, {
    method: "PUT",
    body: userData,
  });
}

function remove(userId) {
  return apiFetch(`/User/${userId}`, {
    method: "DELETE",
  });
}

// Linking
function linkGenre(userId, genreId) {
  return apiFetch(`/User/${userId}/genres/${genreId}`, {
    method: "POST",
  });
}

function unlinkGenre(userId, genreId) {
  return apiFetch(`/User/${userId}/genres/${genreId}`, {
    method: "DELETE",
  });
}

function linkGame(userId, gameId) {
  return apiFetch(`/User/${userId}/games/${gameId}`, {
    method: "POST",
  });
}

function unlinkGame(userId, gameId) {
  return apiFetch(`/User/${userId}/games/${gameId}`, {
    method: "DELETE",
  });
}

export const UsersAPI = {
  getAll,
  getById,
  getByUsername,
  create,
  update,
  remove,
  linkGenre,
  unlinkGenre,
  linkGame,
  unlinkGame,
};
