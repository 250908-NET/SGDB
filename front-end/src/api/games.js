import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/games");
}

function getById(id) {
  return apiFetch(`/games/${id}`);
}

function create(data) {
  return apiFetch("/games", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

function update(id, data) {
  return apiFetch(`/games/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

function remove(id) {
  return apiFetch(`/games/${id}`, { method: "DELETE" });
}

export const GamesAPI = {
  getAll,
  getById,
  create,
  update,
  remove,
};
