import { apiFetch } from "./client";

// GET all users
function getAll() {
  return apiFetch("/user");
}

// GET by ID
function getById(userId) {
  return apiFetch(`/user/${userId}`);
}

// GET by username
function getByUsername(username) {
  return apiFetch(`/user/username/${encodeURIComponent(username)}`);
}

// CREATE user
function create(userData) {
  return apiFetch("/user", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(userData),
  });
}

// UPDATE user
function update(userId, userData) {
  return apiFetch(`/user/${userId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(userData),
  });
}

// DELETE user
function remove(userId) {
  return apiFetch(`/user/${userId}`, {
    method: "DELETE",
  });
}

// LINKING logic
function linkGenre(userId, genreId) {
  return apiFetch(`/user/${userId}/genres/${genreId}`, {
    method: "POST",
  });
}

function unlinkGenre(userId, genreId) {
  return apiFetch(`/user/${userId}/genres/${genreId}`, {
    method: "DELETE",
  });
}

function linkGame(userId, gameId) {
  return apiFetch(`/user/${userId}/games/${gameId}`, {
    method: "POST",
  });
}

function unlinkGame(userId, gameId) {
  return apiFetch(`/user/${userId}/games/${gameId}`, {
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
