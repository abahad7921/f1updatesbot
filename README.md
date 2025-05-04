# 🏎️ F1 Updates Bot

A real-time Formula 1 Discord bot that delivers live race updates, driver lineups, lap-by-lap standings, and final race results. Built with C# and .NET, it integrates seamlessly with the OpenF1 API and the official F1 calendar.

---

## 🚀 Features

* **Live Race Updates**: Automatically posts lap updates during races.
* **Driver Lineups**: Shares the starting grid 30 minutes before each race.
* **Final Standings**: Publishes final race standings immediately after race completion.
* **Scheduled Automation**: Utilizes the official F1 calendar to schedule updates.
* **Discord Integration**: Sends messages to specified Discord channels using Discord.NET.

---

## 🛠️ Setup & Configuration

### Prerequisites

* .NET 6.0 SDK or later
* Discord Bot Token
* Access to the OpenF1 API
* Docker (optional, for containerized deployment)([Akamai][1], [secretgeek.net][2])

### Clone the Repository

```bash
git clone https://github.com/yourusername/f1updatesbot.git
cd f1updatesbot
```

### Configure Environment Variables

Create a `.env` file in the root directory and add the following:

```env
DISCORD_TOKEN=your_discord_bot_token
CHANNEL_ID=your_discord_channel_id
```

Replace the placeholder values with your actual credentials.

---

## 🧪 Running the Bot Locally

Ensure you have the .NET SDK installed on your system.

1. **Restore Dependencies and Build the Project**

   ```bash
   dotnet build
   ```

2. **Run the Application**

   ```bash
   dotnet run
   ```

The bot will start and connect to your specified Discord channel, ready to provide race updates.
---

## 🐳 Running the Bot with Docker

### Build and Run the Docker Container

1. **Build the Docker Image**

   Ensure you have Docker installed and running on your system.

   ```bash
   docker build -t f1updatesbot .
   ```

2. **Run the Docker Container**

   You can pass environment variables directly using the `-e` flag:

   ```bash
   docker run -d \
     --name f1updatesbot \
     -e DISCORD_TOKEN=your_discord_bot_token \
     -e CHANNEL_ID=your_discord_channel_id \
     f1-discord-bot
   ```

   Alternatively, you can use a `.env` file:

   ```bash
   docker run -d \
     --name f1updatesbot \
     --env-file .env \
     f1-discord-bot
   ```

The bot will run inside a Docker container and connect to your specified Discord channel.

---

## 📅 Race Scheduling

The bot automatically schedules updates based on the official F1 calendar. It fetches upcoming race sessions and sets timers for:

* **Driver Lineup**: 30 minutes before race start.
* **Lap Updates**: At race start, every 10 minutes.
* **Final Standings**: At race end.

For the latest race schedule, refer to the [official F1 calendar](https://www.formula1.com/en/racing/2025).

---

## 🤝 Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---