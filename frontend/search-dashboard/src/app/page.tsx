"use client";

import { useMemo, useState } from "react";

type ContentType = "Video" | "Text";

type ContentItem = {
  id: string;
  title: string;
  type: ContentType;
  finalScore: number;
  popularityScore: number;
  relevanceScore: number;
};

type ApiResult = {
  items: ContentItem[];
  page: number;
  pageSize: number;
  totalCount: number;
};

const apiBase = process.env.NEXT_PUBLIC_API_BASE ?? "http://localhost:5018";

export default function Home() {
  const [keyword, setKeyword] = useState("");
  const [type, setType] = useState("");
  const [sortBy, setSortBy] = useState("final");
  const [page, setPage] = useState(1);
  const [result, setResult] = useState<ApiResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const totalPages = useMemo(() => {
    if (!result) return 1;
    return Math.max(1, Math.ceil(result.totalCount / result.pageSize));
  }, [result]);

  const runSearch = async (nextPage = 1) => {
    try {
      setLoading(true);
      setError(null);
      setPage(nextPage);
      const params = new URLSearchParams({
        keyword,
        sortBy,
        page: String(nextPage),
        pageSize: "10",
      });
      if (type) params.set("type", type);

      const response = await fetch(`${apiBase}/api/contents?${params.toString()}`);
      if (!response.ok) throw new Error("Arama istegi basarisiz oldu.");
      const data: ApiResult = await response.json();
      setResult(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Bilinmeyen hata");
    } finally {
      setLoading(false);
    }
  };

  const runSync = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(`${apiBase}/api/contents/sync`, { method: "POST" });
      if (!response.ok) throw new Error("Provider sync basarisiz oldu.");
      await runSearch(1);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Bilinmeyen hata");
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="container">
      <h1>Arama Motoru Dashboard</h1>

      <section className="controls">
        <input
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
          placeholder="Anahtar kelime"
        />
        <select value={type} onChange={(e) => setType(e.target.value)}>
          <option value="">Tum tipler</option>
          <option value="Video">Video</option>
          <option value="Text">Metin</option>
        </select>
        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="final">Final skoru</option>
          <option value="popularity">Populerlik</option>
          <option value="relevance">Alakalilik</option>
        </select>
        <button onClick={() => runSearch(1)} disabled={loading}>
          Ara
        </button>
        <button onClick={runSync} disabled={loading}>
          Provider Sync
        </button>
      </section>

      {error && <p className="error">{error}</p>}

      <table>
        <thead>
          <tr>
            <th>Baslik</th>
            <th>Tip</th>
            <th>Final Skor</th>
            <th>Populerlik</th>
            <th>Alakalilik</th>
          </tr>
        </thead>
        <tbody>
          {result?.items?.map((item) => (
            <tr key={item.id}>
              <td>{item.title}</td>
              <td>{item.type}</td>
              <td>{item.finalScore.toFixed(2)}</td>
              <td>{item.popularityScore.toFixed(2)}</td>
              <td>{item.relevanceScore.toFixed(2)}</td>
            </tr>
          ))}
          {!result?.items?.length && (
            <tr>
              <td colSpan={5}>Kayit bulunamadi. Once sync sonra ara yapin.</td>
            </tr>
          )}
        </tbody>
      </table>

      <section className="pagination">
        <button disabled={page <= 1 || loading} onClick={() => runSearch(page - 1)}>
          Onceki
        </button>
        <span>
          Sayfa {page} / {totalPages}
        </span>
        <button disabled={page >= totalPages || loading} onClick={() => runSearch(page + 1)}>
          Sonraki
        </button>
      </section>
    </main>
  );
}
