import { apiFetch } from "./client";

export const PlatformsAPI = {
  getAll() {
    return apiFetch("/Platforms");            
  },
  getById(id) {
    return apiFetch(`/Platforms/${id}`);      
  },
};
