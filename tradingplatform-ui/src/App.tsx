import { useMemo, useState } from "react";
import axios, { AxiosError } from "axios";
import "./App.css";

type OrderSide = "Buy" | "Sell";
type OrderType = "Market" | "Limit";

type PlaceOrderRequest = {
    instrumentId: string;
    side: OrderSide;
    type: OrderType;
    quantity: number;
    limitPrice: number | null;
};

type PlaceOrderEnvelope = {
    request: PlaceOrderRequest;
};

type PlaceOrderResponse = {
    orderId: string;
    status: string;
    quantity: number;
    filledQuantity: number;
    remainingQuantity: number;
};

type PositionRow = {
    accountId: string;
    instrumentId: string;
    quantity: number;
    avgPrice: number;
    marketPrice: number;
    unrealizedPnl: number;
    updatedAtUtc: string;
};

const API = "https://localhost:7047"; // backend base url

const DEFAULT_ACCOUNT_ID = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
const DEFAULT_INSTRUMENT_ID = "11111111-1111-1111-1111-111111111111";

function toErrorText(err: unknown): string {
    if (err instanceof AxiosError) {
        const data = err.response?.data ?? err.message ?? "Unknown error";
        if (typeof data === "string") return data;
        try {
            return JSON.stringify(data, null, 2);
        } catch {
            return String(data);
        }
    }

    if (err instanceof Error) return err.message;
    return "Unknown error";
}

function fmt(n: number, digits = 2): string {
    if (!Number.isFinite(n)) return String(n);
    return n.toFixed(digits);
}

export default function App() {
    const [accountId, setAccountId] = useState(DEFAULT_ACCOUNT_ID);
    const [instrumentId, setInstrumentId] = useState(DEFAULT_INSTRUMENT_ID);
    const [side, setSide] = useState<OrderSide>("Buy");
    const [type, setType] = useState<OrderType>("Limit");
    const [quantity, setQuantity] = useState<number>(10);
    const [limitPrice, setLimitPrice] = useState<number>(100);

    const [positions, setPositions] = useState<PositionRow[]>([]);
    const [raw, setRaw] = useState<string>("(no output yet)");
    const [showRaw, setShowRaw] = useState<boolean>(true);
    const [loading, setLoading] = useState<boolean>(false);

    const positionsCount = positions.length;

    const totalQty = useMemo(() => {
        return positions.reduce((sum, p) => sum + (p.quantity ?? 0), 0);
    }, [positions]);

    const totalUnrealized = useMemo(() => {
        return positions.reduce((sum, p) => sum + (p.unrealizedPnl ?? 0), 0);
    }, [positions]);

    async function loadPositions(): Promise<void> {
        setLoading(true);
        try {
            const res = await axios.get<PositionRow[]>(
                `${API}/api/positions/${accountId}`
            );
            setPositions(res.data);
            setRaw(JSON.stringify(res.data, null, 2));
        } catch (err: unknown) {
            setRaw(toErrorText(err));
        } finally {
            setLoading(false);
        }
    }

    async function placeOrder(): Promise<void> {
        setLoading(true);
        try {
            const body: PlaceOrderEnvelope = {
                request: {
                    instrumentId,
                    side,
                    type,
                    quantity,
                    limitPrice: type === "Limit" ? limitPrice : null,
                },
            };

            const res = await axios.post<PlaceOrderResponse>(
                `${API}/api/orders/${accountId}`,
                body,
                { headers: { "Content-Type": "application/json" } }
            );

            setRaw(JSON.stringify(res.data, null, 2));

            // Refresh positions after order
            await loadPositions();
        } catch (err: unknown) {
            setRaw(toErrorText(err));
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ padding: 28, maxWidth: 1200, margin: "0 auto" }}>
            <h1 style={{ margin: "10px 0 6px", fontSize: 44, letterSpacing: 0.5 }}>
                Trading Platform UI
            </h1>
            <div style={{ opacity: 0.8, marginBottom: 18 }}>
                Backend: <code>{API}</code>
            </div>

            <div style={{ display: "grid", gridTemplateColumns: "1.2fr 2fr", gap: 18 }}>
                {/* LEFT: Order Ticket */}
                <div
                    style={{
                        borderRadius: 14,
                        padding: 18,
                        background: "rgba(255,255,255,0.04)",
                        boxShadow: "0 10px 30px rgba(0,0,0,0.25)",
                    }}
                >
                    <h2 style={{ marginTop: 0, marginBottom: 12 }}>Order Ticket</h2>

                    <label style={{ display: "block", marginBottom: 6, opacity: 0.85 }}>
                        AccountId
                    </label>
                    <input
                        value={accountId}
                        onChange={(e) => setAccountId(e.target.value)}
                        style={inputStyle}
                    />

                    <label style={{ display: "block", margin: "14px 0 6px", opacity: 0.85 }}>
                        InstrumentId
                    </label>
                    <input
                        value={instrumentId}
                        onChange={(e) => setInstrumentId(e.target.value)}
                        style={inputStyle}
                    />

                    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12, marginTop: 14 }}>
                        <div>
                            <label style={{ display: "block", marginBottom: 6, opacity: 0.85 }}>
                                Side
                            </label>
                            <select
                                value={side}
                                onChange={(e) => setSide(e.target.value as OrderSide)}
                                style={inputStyle}
                            >
                                <option value="Buy">Buy</option>
                                <option value="Sell">Sell</option>
                            </select>
                        </div>

                        <div>
                            <label style={{ display: "block", marginBottom: 6, opacity: 0.85 }}>
                                Type
                            </label>
                            <select
                                value={type}
                                onChange={(e) => setType(e.target.value as OrderType)}
                                style={inputStyle}
                            >
                                <option value="Market">Market</option>
                                <option value="Limit">Limit</option>
                            </select>
                        </div>
                    </div>

                    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12, marginTop: 14 }}>
                        <div>
                            <label style={{ display: "block", marginBottom: 6, opacity: 0.85 }}>
                                Quantity
                            </label>
                            <input
                                type="number"
                                value={quantity}
                                onChange={(e) => setQuantity(Number(e.target.value))}
                                style={inputStyle}
                            />
                        </div>

                        <div>
                            <label style={{ display: "block", marginBottom: 6, opacity: 0.85 }}>
                                Limit Price
                            </label>
                            <input
                                type="number"
                                value={limitPrice}
                                onChange={(e) => setLimitPrice(Number(e.target.value))}
                                style={inputStyle}
                                disabled={type !== "Limit"}
                            />
                        </div>
                    </div>

                    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12, marginTop: 16 }}>
                        <button
                            onClick={() => void placeOrder()}
                            disabled={loading}
                            style={btnPrimary}
                        >
                            {loading ? "Working..." : "Place Order"}
                        </button>

                        <button
                            onClick={() => void loadPositions()}
                            disabled={loading}
                            style={btnSecondary}
                        >
                            {loading ? "Loading..." : "Load Positions"}
                        </button>
                    </div>

                    <div style={{ marginTop: 14, opacity: 0.75, fontSize: 13 }}>
                        Note: For <b>Market</b> orders, Limit Price is automatically sent as <code>null</code>.
                    </div>
                </div>

                {/* RIGHT: Positions */}
                <div>
                    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 12, marginBottom: 14 }}>
                        <SummaryCard title="Positions" value={String(positionsCount)} />
                        <SummaryCard title="Total Qty" value={fmt(totalQty, 0)} />
                        <SummaryCard
                            title="Unrealized PnL"
                            value={fmt(totalUnrealized, 2)}
                            valueColor={totalUnrealized >= 0 ? "#4ade80" : "#f87171"}
                        />
                    </div>

                    <div
                        style={{
                            borderRadius: 14,
                            padding: 18,
                            background: "rgba(255,255,255,0.04)",
                            boxShadow: "0 10px 30px rgba(0,0,0,0.25)",
                        }}
                    >
                        <div style={{ display: "flex", alignItems: "baseline", justifyContent: "space-between" }}>
                            <h2 style={{ marginTop: 0, marginBottom: 12 }}>Positions</h2>
                            <div style={{ opacity: 0.75, fontSize: 12 }}>
                                GET <code>/api/positions/{accountId}</code>
                            </div>
                        </div>

                        <div style={{ overflowX: "auto" }}>
                            <table style={{ width: "100%", borderCollapse: "collapse" }}>
                                <thead>
                                    <tr style={{ textAlign: "left", opacity: 0.85 }}>
                                        <th style={th}>InstrumentId</th>
                                        <th style={th}>Qty</th>
                                        <th style={th}>AvgPrice</th>
                                        <th style={th}>Market</th>
                                        <th style={th}>PnL</th>
                                        <th style={th}>UpdatedAtUtc</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {positions.map((p) => (
                                        <tr key={`${p.accountId}-${p.instrumentId}`} style={{ borderTop: "1px solid rgba(255,255,255,0.08)" }}>
                                            <td style={tdMono}>{p.instrumentId}</td>
                                            <td style={td}>{fmt(p.quantity, 0)}</td>
                                            <td style={td}>{fmt(p.avgPrice, 2)}</td>
                                            <td style={td}>{fmt(p.marketPrice, 2)}</td>
                                            <td style={{ ...td, color: p.unrealizedPnl >= 0 ? "#4ade80" : "#f87171" }}>
                                                {fmt(p.unrealizedPnl, 2)}
                                            </td>
                                            <td style={td}>{p.updatedAtUtc}</td>
                                        </tr>
                                    ))}
                                    {positions.length === 0 && (
                                        <tr>
                                            <td colSpan={6} style={{ padding: "14px 0", opacity: 0.7 }}>
                                                No positions loaded.
                                            </td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>

                        <div style={{ marginTop: 16, display: "flex", alignItems: "center", gap: 10 }}>
                            <button
                                style={btnSecondary}
                                onClick={() => setShowRaw((v) => !v)}
                            >
                                {showRaw ? "Hide Raw Output" : "Show Raw Output"}
                            </button>
                            <div style={{ opacity: 0.7, fontSize: 12 }}>
                                Useful for debugging API responses.
                            </div>
                        </div>

                        {showRaw && (
                            <>
                                <h3 style={{ marginTop: 18, marginBottom: 10 }}>Raw Output</h3>
                                <pre style={rawBox}>{raw}</pre>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

/** Small components + styles */

function SummaryCard(props: {
    title: string;
    value: string;
    valueColor?: string;
}) {
    const { title, value, valueColor } = props;
    return (
        <div
            style={{
                borderRadius: 14,
                padding: 16,
                background: "rgba(255,255,255,0.04)",
                boxShadow: "0 10px 30px rgba(0,0,0,0.25)",
            }}
        >
            <div style={{ opacity: 0.75, fontSize: 13, marginBottom: 6 }}>{title}</div>
            <div style={{ fontSize: 28, fontWeight: 800, color: valueColor ?? "inherit" }}>
                {value}
            </div>
        </div>
    );
}

const inputStyle: React.CSSProperties = {
    width: "100%",
    padding: "10px 12px",
    borderRadius: 10,
    border: "1px solid rgba(255,255,255,0.15)",
    background: "rgba(0,0,0,0.25)",
    color: "white",
    outline: "none",
};

const btnPrimary: React.CSSProperties = {
    padding: "12px 14px",
    borderRadius: 12,
    border: "1px solid rgba(255,255,255,0.15)",
    background: "rgba(0,0,0,0.55)",
    color: "white",
    fontWeight: 800,
    cursor: "pointer",
};

const btnSecondary: React.CSSProperties = {
    padding: "12px 14px",
    borderRadius: 12,
    border: "1px solid rgba(255,255,255,0.15)",
    background: "rgba(255,255,255,0.05)",
    color: "white",
    fontWeight: 700,
    cursor: "pointer",
};

const th: React.CSSProperties = {
    padding: "10px 8px",
    fontWeight: 700,
};

const td: React.CSSProperties = {
    padding: "10px 8px",
    opacity: 0.95,
};

const tdMono: React.CSSProperties = {
    ...td,
    fontFamily: "ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace",
    fontSize: 12,
};

const rawBox: React.CSSProperties = {
    background: "rgba(0,0,0,0.55)",
    borderRadius: 14,
    padding: 14,
    overflowX: "auto",
    boxShadow: "inset 0 0 0 1px rgba(255,255,255,0.06)",
    color: "#a7f3d0",
};
