import { apiFetch } from "./client";

export function getByUser(userId) {
  return apiFetch(`/Rating/user/${userId}`);
}

export function getOne(userId, gameId) {
  return apiFetch(`/Rating/${userId}/${gameId}`);
}

export function create(dto) {
  return apiFetch(`/Rating`, {
    method: "POST",
    body: dto,
  });
}

export function update(userId, gameId, dto) {
  return apiFetch(`/Rating/${userId}/${gameId}`, {
    method: "PUT",
    body: dto,
  });
}

export function remove(userId, gameId) {
  return apiFetch(`/Rating/${userId}/${gameId}`, {
    method: "DELETE",
  });
}
