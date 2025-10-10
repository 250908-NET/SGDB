import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/platforms");
}

function getById(id) {
  return apiFetch(`/platforms/${id}`);
}

function create(data) {
  return apiFetch("/platforms", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

function update(id, data) {
  return apiFetch(`/platforms/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

function remove(id) {
  return apiFetch(`/platforms/${id}`, { method: "DELETE" });
}

export const PlatformsAPI = {
  getAll,
  getById,
  create,
  update,
  remove,
};
