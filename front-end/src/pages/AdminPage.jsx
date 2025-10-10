import { useEffect, useState } from "react";
import { GamesAPI } from "../api/games";
import { CompaniesAPI } from "../api/companies";
import { GenresAPI } from "../api/genres";
import { PlatformsAPI } from "../api/platforms";
import GamesTab from "./tabs/GamesTab";
import GenresTab from "./tabs/GenresTab";
import PlatformsTab from "./tabs/PlatformsTab";
import CompaniesTab from "./tabs/CompaniesTab";

export default function AdminPage() {
  const [activeTab, setActiveTab] = useState("games");

  const [games, setGames] = useState([]);
  const [companies, setCompanies] = useState([]);
  const [genres, setGenres] = useState([]);
  const [platforms, setPlatforms] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    loadAll();
  }, []);

  async function loadAll() {
    setLoading(true);
    try {
      const [gamesData, companiesData, genresData, platformsData] = await Promise.all([
        GamesAPI.getAll(),
        CompaniesAPI.getAll(),
        GenresAPI.getAll(),
        PlatformsAPI.getAll(),
      ]);
      setGames(gamesData || []);
      setCompanies(companiesData || []);
      setGenres(genresData || []);
      setPlatforms(platformsData || []);
      setError("");
    } catch (err) {
      console.error(err);
      setError("Failed to load data.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ padding: 24, maxWidth: 1000, margin: "0 auto" }}>
      <h2>Admin Dashboard</h2>

      {/* Tabs */}
      <div style={{ display: "flex", gap: 12, marginBottom: 20 }}>
        <button
          onClick={() => setActiveTab("games")}
          style={{
            padding: "8px 16px",
            borderRadius: 6,
            border: "none",
            background: activeTab === "games" ? "#2563eb" : "#e5e7eb",
            color: activeTab === "games" ? "white" : "black",
            cursor: "pointer",
          }}
        >
          Games
        </button>

        <button
          onClick={() => setActiveTab("genres")}
          style={{
            padding: "8px 16px",
            borderRadius: 6,
            border: "none",
            background: activeTab === "genres" ? "#2563eb" : "#e5e7eb",
            color: activeTab === "genres" ? "white" : "black",
            cursor: "pointer",
          }}
        >
          Genres
        </button>

        <button
          onClick={() => setActiveTab("platforms")}
          style={{
            padding: "8px 16px",
            borderRadius: 6,
            border: "none",
            background: activeTab === "platforms" ? "#2563eb" : "#e5e7eb",
            color: activeTab === "platforms" ? "white" : "black",
            cursor: "pointer",
          }}
        >
          Platforms
        </button>

        <button
          onClick={() => setActiveTab("companies")}
          style={{
            padding: "8px 16px",
            borderRadius: 6,
            border: "none",
            background: activeTab === "companies" ? "#2563eb" : "#e5e7eb",
            color: activeTab === "companies" ? "white" : "black",
            cursor: "pointer",
          }}
        >
          Companies
        </button>
      </div>

      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}
      {loading && <div style={{ color: "#555" }}>Loading...</div>}

      {activeTab === "games" && (
        <GamesTab
          games={games}
          companies={companies}
          genres={genres}
          platforms={platforms}
          reload={loadAll}
        />
      )}
      {activeTab === "genres" && <GenresTab genres={genres} reload={loadAll} />}
      {activeTab === "platforms" && <PlatformsTab platforms={platforms} reload={loadAll} />}
      {activeTab === "companies" && <CompaniesTab companies={companies} reload={loadAll} />}
    </div>
  );
}
