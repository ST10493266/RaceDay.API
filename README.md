# RaceDay.API
# RaceDay Cloud-Native Event Management Platform

YouTube link:
https://youtu.be/tYnAPrphtP0 

An integrated, role-aware, multi-tiered tournament orchestration management system built explicitly using **ASP.NET Core 8.0**. 

The repository is structured into a clean architectural pattern: a backend core engine exposing decoupled RESTful endpoints (`RaceDay.Api`) and a secure, non-database-coupled web frontend client (`RaceDay.Web`).

---

##  System Architecture Layout

The platform avoids direct database leakage or presentation layer duplication by passing typed DTO models back and forth over asynchronous HTTP streams:

```text
RaceDay/                             # Root Solution Directory Tree
├── RaceDay.sln                      # Solution Configuration Orchestrator
├── .github/
│   └── workflows/
│       └── ci.yml                   # Automated GitHub Actions Workflow Engine
├── src/
│   ├── RaceDay.Api/                 # Core API Engine (Part 2 Payload Store)
│   └── RaceDay.Web/                 # Role-Aware MVC Client App Gateway
└── tests/
    ├── RaceDay.Api.Tests/           # Unmodified Backend Logic Mock Tests
    └── RaceDay.Web.Tests/           # Frontend Proxy Isolation Validation Harness
```

---

##  Key Feature Sets

###  1. Role-Aware Sessions
*   **Organisers:** Access a secure management workspace dashboard allowing them to design new racing entries and process race field statistics.
*   **Participants:** Browse community events through dynamic filtering layout views and view historical placing and timing metrics.

###  2. Safe Interface Guardians
Frontend routing paths are locked via a custom filter attribute model (`SessionGuardAttribute.cs`). Unauthorized users or mismatched permission queries are immediately bounced out to the authentication portal before the markup layer loads.

###  3. Decoupled Media Delivery Stream
The frontend layer maintains a zero-knowledge stance regarding third-party service connections. Banner assets uploaded as multi-part form payloads are streamed over a data pipeline into the API backend layer, where they are mapped and written out safely to **Azure Blob Storage**.

---

##  Local System Initialization & Setup

### Requirements
*   [.NET 8.0 Software Development Kit (SDK)](https://microsoft.com)
*   [Docker Desktop Engine](https://docker.com)

### Step 1: Solution Package Resolution & Compilation
Open a terminal prompt inside the project root workspace folder and build the solutions:
```bash
# Pull dependencies
dotnet restore RaceDay.sln

# Compile all projects simultaneously
dotnet build RaceDay.sln --configuration Release
```

### Step 2: Running Automated Unit Verification Tests
Execute both test projects inside the test runner assembly loop to verify state and filter accuracy:
```bash
dotnet test RaceDay.sln --configuration Release --verbosity normal
```

---

##  Container Infrastructure Instructions

The presentation layer utilizes optimized, multi-stage compilation patterns inside its isolated deployment asset definition file (`Dockerfile`).

### Option A: Local Build and Execution (Single Command Loop)
To compile the frontend client container and dynamically point its request proxy engine down toward your locally operating backend core endpoint, run this terminal execution statement:

```bash
docker build -t raceday-web-frontend ./src/RaceDay.Web && \
docker run -d \
  -p 8080:8080 \
  --name web-client \
  -e "BackendApi__BaseUrl=http://docker.internal" \
  raceday-web-frontend
```

### Option B: Verification Targets
Once initiated inside your host's engine loops, explore these runtime application nodes:
*   **Web Client Portal Address:** `http://localhost:8080`
*   **Secure API Endpoint Gateway Target:** `http://localhost:5000/api/`

---

##  Continuous Integration Pipeline

An automated verification configuration model is registered under **`.github/workflows/ci.yml`**. On every pull request or direct branch push targeted at your repository's production branches, it kicks off a headless environment executing these automated operational steps:
1. Installs the hosted **Ubuntu Linux** runner and registers the proper **.NET 8.0 Runtime Framework**.
2. Executes complete workspace dependency restorations via NuGet.
3. Builds the total compilation package under production optimization routines (`-c Release`).
4. Runs **both testing harnesses** (`RaceDay.Api.Tests` & `RaceDay.Web.Tests`) in parallel to safeguard system integrity.
