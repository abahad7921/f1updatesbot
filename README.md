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

* .NET 8.0 SDK or later
* Discord Bot Token
* Access to the OpenF1 API
* Docker (optional, for containerized deployment)

### Clone the Repository

```bash
git clone https://github.com/yourusername/f1updatesbot.git
cd f1updatesbot
```

### Get the Discord Channel ID

1. In Discord desktop or web, click **⚙️ User Settings**.
2. Go to **Advanced** and turn **Developer Mode** on.
3. Return to the server containing the channel that should receive updates.
4. Right-click that channel and choose **Copy Channel ID**.

The copied value will look similar to `123456789012345678`. Use it as your `CHANNEL_ID`.

### Get the Bot Token

If you have already created a Discord application:

1. Open the [Discord Developer Portal](https://discord.com/developers/applications).
2. Select your application and open **Bot** from the left sidebar.
3. Under **Token**, choose **Reset Token** (or generate a token if prompted).
4. Copy the token immediately and store it securely. Discord will not show the full token again.

Treat the token like a password: never commit it to Git, share it, or include it in screenshots. If it is exposed, reset it in the Developer Portal.

### Configure Environment Variables

Create a `.env` file in the root directory and add the following:

```env
BOT_TOKEN=your_discord_bot_token
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
     -e BOT_TOKEN=your_discord_bot_token \
     -e CHANNEL_ID=your_discord_channel_id \
     f1updatesbot
   ```

   Alternatively, you can use a `.env` file:

   ```bash
   docker run -d \
     --name f1updatesbot \
     --env-file .env \
     f1updatesbot
   ```

The bot will run inside a Docker container and connect to your specified Discord channel.

---

## 📅 Race Scheduling

The bot automatically schedules updates based on the official F1 calendar. It fetches upcoming race sessions and sets timers for:

* **Driver Lineup**: 30 minutes before race start.
* **Lap Updates**: At race start, every 10 minutes.
* **Final Standings**: At race end.

For the latest race schedule, refer to the [official F1 calendar](https://www.formula1.com/en/racing).

---

## 🤝 Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---
