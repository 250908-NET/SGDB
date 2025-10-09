import { apiFetch } from "./client";


export const CompaniesAPI = {
  getAll() {
    return apiFetch("/Company");          
  },
  getById(id) {
    return apiFetch(`/Company/${id}`);    
  },
};
