import { apiFetch } from "./client";

function getAll() {
  return apiFetch("/company");
}

function getById(id) {
  return apiFetch(`/company/${id}`);
}

function create(data) {
  return apiFetch("/company", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

function update(id, data) {
  return apiFetch(`/company/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

function remove(id) {
  return apiFetch(`/company/${id}`, { method: "DELETE" });
}

export const CompaniesAPI = {
  getAll,
  getById,
  create,
  update,
  remove,
};
