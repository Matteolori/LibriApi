import { useState } from "react";
import { login } from "./api";

interface Props {
  onLogin: (token: string) => void;
}

export default function LoginForm({ onLogin }: Props) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errore, setErrore] = useState<string | null>(null);

  async function invia(e: React.FormEvent) {
    e.preventDefault();
    setErrore(null);
    try {
      onLogin(await login(username, password));
    } catch (err) {
      setErrore(err instanceof Error ? err.message : "Errore sconosciuto");
    }
  }

  return (
    <form onSubmit={invia}>
      <input
        placeholder="Username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />
      <button type="submit">Accedi</button>
      {errore && <p style={{ color: "red" }}>{errore}</p>}
    </form>
  );
}