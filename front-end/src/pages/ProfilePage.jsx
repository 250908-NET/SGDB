import { useApp } from "../context/AppContext";

export default function ProfilePage() {
  const { username } = useApp();
  return (
    <section className="page">
      <h2>Profile</h2>
      <p>Logged in as <strong>{username}</strong>.</p>
    </section>
  );
}
