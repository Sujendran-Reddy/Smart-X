# Smart-X
Smart-X is a simulated IoT gateway for registering sensors, submitting typed telemetry and investigating sensor readings via a Blazor dashboard.

## Technology
- .NET 10 and ASP.NET Core Minimal APIs
- Blazor WebAssembly & Bootstrap styling
- No database or physical hardware, in-memory collections

### SmartX.Api 
- API endpoints
- Validation
- Telemetry
- Storage
- Monitoring

### SmartX.Client
- Browser dashboard
- Forms
- History
- Attachments

### SmartX.Shared
- Models
- Requests
- Shared data

## Run Locally
### Requirements
- .NET 10 SDK

### How to run (Powershell)
1. Clone the repo
2. Open Powershell in the folder containing SmartX.slnx
3. Trust the dev certificate, restore dependencies & build
   - dotnet dev-certs https --trust
   - dotnet restore SmartX.slnx"
   - dotnet build SmartX.slnx --no-restore
4. Start the API
   - dotnet run --project SmartX.Api --launch-profile https
5. Leave that terminal running, open a new terminal
6. Start the Client
   - dotnet run --project SmartX.Client --launch-profile https
7. Open https://localhost:7102 in your browser

### How to Run Visual Studio
1. Install Visual Studio with:
   - ASP.NET
   - Web development workload
   - .NET 10 SDK
2. Clone the repository and open SmartX.slnx
3. In the solution explorer, right click the solution and click configure startup projects.
4. Select multiple projects and set the following:
   - SmartX.Api as Start
   - SmartX.Client as Start
   - SmartX.Shared as None
5. Select Build - Build Solution.
6. Press Crtl + F5 or click the play button, accept the dev certificate prompt if shown.
7. Open https://localhost:7102

### Notes
- API health check: **https://localhost:7101/api/health**
- OpenAPI document (Development): **https://localhost:7101/openapi/v1.json**
- Stop each application with `Ctrl+C` in its terminal.


## Features
- register sensors with a uique device identifier, deployment location, category and data type
- submit and retrieve float, integer and Boolean telemetry, including valid 0 and false values
- search sensors, filter by category and browse pages of 25 sensors.
- ciew history grouped by UTC date using jagged arrays
- upload and download sensor attachments up to 5 MiB. Supported extensions: JSON, XML, YAML, YML, TXT, CSV, LOG, JPG, JPEG, PNG and PDF
- validate nested deployment trees recursively, including sensor registration, duplicate assignments and disabled ancestors. Validation limits are 64 levels and 10,000 visited nodes
- generate 1,000 simulated sensors and 175,010 readings across seven historical days. Repeating a completed seed request returns the original result during the same API run
- monitor Normal, Warning, Stale and No data states with automatic refresh every five seconds, manual refresh and status filters

## Demo
1. Register an Environmental sensor with the Float data type.
2. Open its telemetry page and submit `42.5`.
3. Open daily history and confirm the reading appears under today's UTC date.
4. Open monitoring: the fresh reading should be Normal with the default configuration.
5. Submit `95` and refresh monitoring to see a Warning.
6. Stop submitting readings. After more than 120 seconds, refresh monitoring to see Stale.
7. Upload a small TXT or PNG attachment and download it again.
8. Load the demo dataset to try searching, pagination and multi-day history. Historical seed readings will appear Stale; submit a new reading to demonstrate a fresh status.
