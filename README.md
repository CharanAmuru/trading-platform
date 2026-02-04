📈 Trading Platform – Full-Stack Simulation
A full-stack trading platform simulation built to demonstrate real-world trading workflows including order placement, position management, and PnL calculation, with a modern React UI and a .NET backend.
This project is not a toy UI — it follows real backend-driven flows similar to institutional trading systems.
<img width="1918" height="1001" alt="UI IMAGE" src="https://github.com/user-attachments/assets/3fc71af0-190c-4ab7-84a1-12047662e404" />

🚀 Tech Stack
Frontend
React + TypeScript
Vite
Axios


Modern component-based UI
Reason: fast development, type safety, clean separation of concerns.

Backend
ASP.NET Core Web API
Swagger / OpenAPI
In-memory data simulation (for trading logic)
Reason: enterprise-grade API patterns, clean contracts.

🧩 Features
✅ Order Management
Place Market and Limit orders
Buy / Sell support
Server-validated requests
Real-time updates after order execution

✅ Position Management
Fetch positions per account
Aggregated quantity
Average price calculation
Market price & Unrealized PnL
Last updated timestamp (UTC)

✅ UI Highlights
Trading ticket (Order entry panel)
Positions table

Summary cards:
Total Positions
Total Quantity
Unrealized PnL
Raw API response viewer (debug-friendly)

✅ API-First Design
Fully testable via Swagger UI
Frontend strictly consumes backend APIs
No mock data in UI

🖥️ Screenshots
Screenshots included in the repository show:
Order placement via UI
Live position updates
Swagger request/response validation
Market vs Limit order behavior

🔌 API Endpoints
Place Order
POST /api/orders/{accountId}


Request

{
  "request": {
    "instrumentId": "11111111-1111-1111-1111-111111111111",
    "side": "Buy",
    "type": "Limit",
    "quantity": 10,
    "limitPrice": 100
  }
}


Response

{
  "orderId": "2b2170d7-95ef-4e40-af0a-9f53c50a2c0f",
  "status": "Accepted",
  "quantity": 10,
  "filledQuantity": 0,
  "remainingQuantity": 10
}

Get Positions
GET /api/positions/{accountId}



🛠️ How to Run Locally
Backend
dotnet run


Swagger available at:
https://localhost:7047/swagger

Frontend
npm install
npm run dev


UI available at:
http://localhost:5174

📌 Project Purpose

This project was built to:

Demonstrate full-stack engineering skills  

Show real API-driven UI integration

Practice clean architecture & typing

Provide a portfolio-ready trading system simulation

⚠️ This is a simulation — no real market connectivity or financial execution.

🧠 Future Enhancements
Authentication & user accounts
WebSocket live price updates
Persistent database (SQL Server)
Order lifecycle states (Filled, Cancelled)
Risk checks & validations


👤 Author
Charan Amuru
Full-Stack / Backend-leaning Engineer
Master’s in Computer Science

⭐ If you like this project
Star ⭐ the repository and feel free to fork or explore.
