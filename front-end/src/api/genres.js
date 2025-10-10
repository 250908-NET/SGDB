import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/Genres");
}

function getById(id) {
  return apiFetch(`/genres/${id}`);
}

function create(data) {
  return apiFetch("/genres", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

function update(id, data) {
  return apiFetch(`/genres/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

function remove(id) {
  return apiFetch(`/genres/${id}`, { method: "DELETE" });
}

export const GenresAPI = {
  getAll,
  getById,
  create,
  update,
  remove,
};

