# 🌍 EcoSnap - Smart Waste Management System

### 🎓 Graduation Project - Class of 2026

**Menoufia University - Faculty of Science**  
_Department of Mathematics and Computer Science_

---

## 📝 About the Project

**EcoSnap** is an innovative, AI-powered smart waste management ecosystem designed to revolutionize plastic recycling through computer vision and community rewards. The platform automates bottle identification via modern mobile clients, awards users points instantly, and streamlines the operational workflow for recyclers, digital drivers, and administrators.

---

## 🚀 Key Features

### 👤 User Application (Mobile Client)

- **AI Bottle Identification:** Scan and detect plastic bottles instantly using computer vision models.
- **Dynamic Wallet System:** Track earned points live with an optimized local ledger.
- **Instant Cash Redemptions:** Seamless bypass integration mimicking real-time digital wallet payouts (e.g., Paymob CashOut / Vodafone Cash).
- **Driver Feedback System:** Rate your digital recycling driver and provide trip feedback securely via robust Async/Task endpoints.

### 🚛 Recycler & Digital Driver Hub

- **Trip Management:** Accept, manage, and complete pending localized pickup requests.
- **Automated Verification:** Update bottle counts and instantly trigger wallet credits to user accounts upon collection.

### 🛡️ Admin Dashboard (Management Portal)

- **Advanced User Filters:** Query users dynamically by name, email, activity status, and accumulated metrics.
- **Dynamic Leaderboards:** Rank users automatically based on bottle counts and overall recycling milestones.
- **Manual Redemption Overrides:** Fully track and approve financial operations directly from a centralized back-office panel.

---

## 🛠️ Tech Stack & Architecture

The system utilizes a modern, distributed architecture built with industry-standard patterns:

| Layer / Component      | Technology Used                                                                  |
| :--------------------- | :------------------------------------------------------------------------------- |
| **Backend Framework**  | **.NET 8 Web API** (ASP.NET Core C#)                                             |
| **Database & ORM**     | **SQL Server** + **Entity Framework Core** (Database-First Model)                |
| **Authentication**     | **JWT (JSON Web Tokens)** Role-Based Authorization (`User`, `Recycler`, `Admin`) |
| **Third-Party APIs**   | **Paymob Gateway** Integration Framework (HMAC SHA512 security validation)       |
| **API Documentation**  | **Swagger UI** with rich OpenAPI annotations                                     |
| **Client Application** | **Flutter** (Cross-platform Android & iOS)                                       |

---

## 📂 Backend Architecture Highlights

Our .NET backend is built from the ground up prioritizing scalability, clean code principles, and bulletproof security:

- **Repository Pattern:** Fully decoupling data access from business logic via structured interfaces (`IUserRepository`, `IPaymentService`).
- **Asynchronous Execution:** End-to-end `async/await` Task pipeline preventing background context disposal and optimizing thread utilization.
- **Security & HMAC Validation:** Bulletproof integration validation utilizing strict SHA512 hashing to authenticate callback webhooks safely.
- **Robust Error Handling:** Global middleware protections catching key mismatches and preventing server exposure.

---

## ⚙️ How to Run the Backend Locally

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) & SSMS

### Setup Instructions

1. **Clone the Repository:**
   ```bash
   git clone [https://github.com/your-username/Smart-Waste-.git](https://github.com/your-username/Smart-Waste-.git)
   cd Smart-Waste-
   ```
