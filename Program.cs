using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

class DiscordTool
{
    // Global configuration loaded from JSON (or defaults)
    static Config config = LoadConfig();

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Set window colors from config (if desired)
        Console.ForegroundColor = config.DefaultForeground;
        Console.BackgroundColor = config.DefaultBackground;
        Console.Clear();

        Banner();
        await MainLoop();
    }

    /// <summary>
    /// Main loop that uses arrow-key navigation to choose an option.
    /// </summary>
    static async Task MainLoop()
    {
        while (true)
        {
            int choice = ShowMenuNavigation();
            try
            {
                // choice is a 0-based index; add 1 for our switch
                switch (choice + 1)
                {
                    case 1:
                        await WebhookSpammer();
                        break;
                    case 2:
                        await SendWebhookMessage();
                        break;
                    case 3:
                        await DeleteWebhook();
                        break;
                    case 4:
                        await CustomEmbedMessage();
                        break;
                    case 5:
                        await MessageScheduler();
                        break;
                    case 6:
                        await FakeScanningEffect();
                        break;
                    case 7:
                        await KickMember();
                        break;
                    case 8:
                        await BanMember();
                        break;
                    case 9:
                        await MassRoleAssignment();
                        break;
                    case 10:
                        TokenGrabber();
                        break;
                    case 11:
                        await MassDMSmammer();
                        break;
                    case 12:
                        AccountChecker();
                        break;
                    case 13:
                        await ServerInfoFetcher();
                        break;
                    case 14:
                        FakeIPAddressGenerator();
                        break;
                    case 15:
                        PasswordStrengthChecker();
                        break;
                    case 16:
                        await NukerMenu();  // Jump to nuker mode menu
                        break;
                    case 17:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid Option. Try Again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    // ************* Banner ******************
    static void Banner()
    {
        Console.Clear();
        // You can change the colors in your config file.
        string[] logoLines = new string[]
        {
            "  ██████╗ ██╗███████╗ ██████╗ ██████╗  ██████╗ ███████╗██████╗ ",
            "  ██╔══██╗██║██╔════╝██╔════╝ ██╔══██╗██╔═══██╗██╔════╝██╔══██╗",
            "  ██████╔╝██║█████╗  ██║  ███╗██████╔╝██║   ██║█████╗  ██████╔╝",
            "  ██╔═══╝ ██║██╔══╝  ██║   ██║██╔═══╝ ██║   ██║██╔══╝  ██╔═══╝ ",
            "  ██║     ██║███████╗╚██████╔╝██║     ╚██████╔╝███████╗██║     ",
            "  ╚═╝     ╚═╝╚══════╝ ╚═════╝ ╚═╝      ╚═════╝ ╚══════╝╚═╝     "
        };

        // Using gradient colors from config (default: Cyan, Cyan, Gray, Gray, White, White)
        ConsoleColor[] gradient = config.BannerGradientColors;

        int top = (Console.WindowHeight - logoLines.Length - 4) / 2;
        for (int i = 0; i < top; i++)
            Console.WriteLine();

        for (int i = 0; i < logoLines.Length; i++)
        {
            Console.ForegroundColor = gradient[i];
            CenterWrite(logoLines[i]);
        }
        Console.ResetColor();
        CenterWrite("Discord Multi-Tool by WtireXoo");
        CenterWrite("=============================\n");
    }

    /// <summary>
    /// Draws the main menu with two columns and uses arrow-key navigation.
    /// Returns the index (0-based) of the selected menu item.
    /// </summary>
    static int ShowMenuNavigation()
    {
        // Define two arrays for the two columns
        var leftMenu = new (string id, string label)[]
        {
            ("(01)", "Webhook Spammer"),
            ("(02)", "Send Webhook Message"),
            ("(03)", "Delete Webhook"),
            ("(04)", "Custom Embed Message"),
            ("(05)", "Message Scheduler"),
            ("(06)", "Fake Scanning Effect"),
            ("(07)", "Kick Member"),
            ("(08)", "Ban Member"),
            ("(09)", "Mass Role Assignment"),
            ("(10)", "Token Grabber"),
            ("(11)", "Mass DM"),
            ("(12)", "Acc Checker")
        };

        var rightMenu = new (string id, string label)[]
        {
            ("(13)", "Server Info Fetcher"),
            ("(14)", "Fake IP Generator"),
            ("(15)", "Password Strength Checker"),
            ("(16)", "Nuker Menu"),
            ("(17)", "Exit")
        };

        // Total options: 12 left + 5 right = 17. The menu is arranged by rows.
        int totalRows = leftMenu.Length; // 12 rows. Right column is only filled for first 5 rows.
        int totalOptions = totalRows + rightMenu.Length; // 17 options.

        int selectedIndex = 0;

        ConsoleKey key;
        do
        {
            Console.Clear();
            // Draw banner (if you want to show it above the menu, call Banner() here)
            Banner();

            // Write the heading for the menu in centered columns
            Console.ForegroundColor = config.MenuHeadingColor;
            string leftHeader = "+-------------------- Discord Tools ---------------------+";
            string rightHeader = "+-------------------- Utility Tools ---------------------+";
            CenterWrite(leftHeader + "   " + rightHeader);
            Console.ResetColor();

            // Render each row. For row i, left menu item is always present.
            // For right, only if i < rightMenu.Length.
            for (int row = 0; row < totalRows; row++)
            {
                // Calculate option numbers:
                int leftOptionIndex = row;         // 0 to 11
                int rightOptionIndex = row + totalRows; // 12 to 16

                // Check if current left/right items are selected
                bool leftSelected = (selectedIndex == leftOptionIndex);
                bool rightSelected = (row < rightMenu.Length && selectedIndex == rightOptionIndex);

                // Set colors accordingly
                Console.ForegroundColor = leftSelected ? config.MenuSelectedForeground : config.MenuForeground;
                Console.BackgroundColor = leftSelected ? config.MenuSelectedBackground : Console.BackgroundColor;
                string leftText = $"| {leftMenu[row].id} {leftMenu[row].label.PadRight(28)}";
                Console.Write(leftText);
                Console.ResetColor();

                Console.Write("   ");

                if (row < rightMenu.Length)
                {
                    Console.ForegroundColor = rightSelected
                        ? config.MenuSelectedForeground
                        : (rightMenu[row].label == "Exit" ? config.MenuExitColor : config.MenuForeground);
                    Console.BackgroundColor = rightSelected ? config.MenuSelectedBackground : Console.BackgroundColor;
                    string rightText = $"| {rightMenu[row].id} {rightMenu[row].label.PadRight(28)}";
                    Console.Write(rightText);
                    Console.ResetColor();

                    Console.WriteLine("   |");
                }
                else
                {
                    // No right item on this row; fill blank space.
                    Console.WriteLine();
                }
            }

            // Write footer line for menu
            Console.ForegroundColor = config.MenuHeadingColor;
            string leftFooter = "+--------------------------------------------------------+";
            string rightFooter = "+--------------------------------------------------------+";
            CenterWrite(leftFooter + "   " + rightFooter);
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNOTE: Any kind of damage caused is not the creator's fault! // Move with arrows.");
            Console.ResetColor();

            // Read key for navigation
            var keyInfo = Console.ReadKey(true);
            key = keyInfo.Key;
            int previousIndex = selectedIndex;

            if (key == ConsoleKey.UpArrow)
            {
                // Move up one option (wrap around)
                selectedIndex = (selectedIndex - 1 + totalOptions) % totalOptions;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                // Move down one option (wrap around)
                selectedIndex = (selectedIndex + 1) % totalOptions;
            }
            else if (key == ConsoleKey.LeftArrow)
            {
                // If currently in a right column option, move to its left partner
                if (selectedIndex >= totalRows)
                    selectedIndex -= totalRows;
            }
            else if (key == ConsoleKey.RightArrow)
            {
                // If a left column item and there is a right column counterpart, jump right
                if (selectedIndex < totalRows && selectedIndex + totalRows < totalOptions)
                    selectedIndex += totalRows;
            }
            // Beep on navigation change
            if (selectedIndex != previousIndex)
                Console.Beep(config.BeepFrequency, config.BeepDuration);
        }
        while (key != ConsoleKey.Enter);

        return selectedIndex;
    }

    /// <summary>
    /// Helper to write text centered in the current console width.
    /// </summary>
    static void CenterWrite(string text)
    {
        int width = Console.WindowWidth;
        int leftPadding = (width - text.Length) / 2;
        Console.WriteLine(new string(' ', Math.Max(0, leftPadding)) + text);
    }

    // ************* JSON Config Loader ******************
    static Config LoadConfig()
    {
        // Default settings
        Config defaultConfig = new Config
        {
            BannerGradientColors = new ConsoleColor[] { ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.Gray, ConsoleColor.Gray, ConsoleColor.White, ConsoleColor.White },
            MenuForeground = ConsoleColor.Cyan,
            MenuHeadingColor = ConsoleColor.White,
            MenuSelectedForeground = ConsoleColor.Black,
            MenuSelectedBackground = ConsoleColor.White,
            MenuExitColor = ConsoleColor.Red,
            DefaultForeground = ConsoleColor.DarkCyan,
            DefaultBackground = ConsoleColor.DarkRed,
            BeepFrequency = 800,
            BeepDuration = 100
        };

        // Load from file if exists
        string configPath = "config.json";
        if (File.Exists(configPath))
        {
            try
            {
                string json = File.ReadAllText(configPath);
                // Deserialize using System.Text.Json (colors are stored as strings)
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var loadedConfig = JsonSerializer.Deserialize<ConfigJson>(json, options);

                // Convert loaded string color names to ConsoleColor
                defaultConfig.BannerGradientColors = loadedConfig.BannerGradientColors.Select(s => (ConsoleColor)Enum.Parse(typeof(ConsoleColor), s, true)).ToArray();
                defaultConfig.MenuForeground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.MenuForeground, true);
                defaultConfig.MenuHeadingColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.MenuHeadingColor, true);
                defaultConfig.MenuSelectedForeground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.MenuSelectedForeground, true);
                defaultConfig.MenuSelectedBackground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.MenuSelectedBackground, true);
                defaultConfig.MenuExitColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.MenuExitColor, true);
                defaultConfig.DefaultForeground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.DefaultForeground, true);
                defaultConfig.DefaultBackground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), loadedConfig.DefaultBackground, true);
                defaultConfig.BeepFrequency = loadedConfig.BeepFrequency;
                defaultConfig.BeepDuration = loadedConfig.BeepDuration;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Config load error: {ex.Message}");
            }
        }
        return defaultConfig;
    }

    // ***************** CONFIG CLASSES *****************
    class Config
    {
        public ConsoleColor[] BannerGradientColors { get; set; }
        public ConsoleColor MenuForeground { get; set; }
        public ConsoleColor MenuHeadingColor { get; set; }
        public ConsoleColor MenuSelectedForeground { get; set; }
        public ConsoleColor MenuSelectedBackground { get; set; }
        public ConsoleColor MenuExitColor { get; set; }
        public ConsoleColor DefaultForeground { get; set; }
        public ConsoleColor DefaultBackground { get; set; }
        public int BeepFrequency { get; set; }
        public int BeepDuration { get; set; }
    }
    // This class matches the JSON structure (colors as strings).
    class ConfigJson
    {
        public string[] BannerGradientColors { get; set; }
        public string MenuForeground { get; set; }
        public string MenuHeadingColor { get; set; }
        public string MenuSelectedForeground { get; set; }
        public string MenuSelectedBackground { get; set; }
        public string MenuExitColor { get; set; }
        public string DefaultForeground { get; set; }
        public string DefaultBackground { get; set; }
        public int BeepFrequency { get; set; }
        public int BeepDuration { get; set; }
    }

    // ********************* Feature Implementations *********************
    // (All your existing feature methods remain unchanged.)
    // 1. Webhook Spammer
    static async Task WebhookSpammer()
    {
        Console.Clear();
        Console.WriteLine("=== Webhook Spammer ===");
        Console.Write("Enter Webhook URL: ");
        string url = Console.ReadLine();
        Console.Write("Enter message to spam: ");
        string message = Console.ReadLine();
        Console.Write("How many times to spam: ");
        int times = int.Parse(Console.ReadLine());

        HttpClient client = new HttpClient();
        for (int i = 0; i < times; i++)
        {
            var content = new StringContent($"{{\"content\":\"{message}\"}}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            Console.WriteLine(response.IsSuccessStatusCode ?
                $"[{i + 1}] Message sent." :
                $"[{i + 1}] Failed. Status: {response.StatusCode}");
            await Task.Delay(2000);
        }
        Console.WriteLine("\nSpam finished. Press Enter to return...");
        Console.ReadLine();
    }

    // 2. Send Webhook Message
    static async Task SendWebhookMessage()
    {
        Console.Clear();
        Console.WriteLine("=== Send Webhook Message ===");
        Console.Write("Enter Webhook URL: ");
        string url = Console.ReadLine();
        Console.Write("Enter your message: ");
        string message = Console.ReadLine();

        HttpClient client = new HttpClient();
        var content = new StringContent($"{{\"content\":\"{message}\"}}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        Console.WriteLine(response.IsSuccessStatusCode ?
            "Message sent successfully." :
            $"Failed. Status: {response.StatusCode}");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 3. Delete Webhook
    static async Task DeleteWebhook()
    {
        Console.Clear();
        Console.WriteLine("=== Webhook Deleter ===");
        Console.Write("Enter Webhook URL: ");
        string url = Console.ReadLine();

        HttpClient client = new HttpClient();
        var response = await client.DeleteAsync(url);
        Console.WriteLine(response.IsSuccessStatusCode ?
            "Webhook deleted successfully." :
            $"Failed to delete. Status: {response.StatusCode}");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 4. Custom Embed Message
    static async Task CustomEmbedMessage()
    {
        Console.Clear();
        Console.WriteLine("=== Custom Embed Message ===");
        Console.Write("Enter Webhook URL: ");
        string url = Console.ReadLine();
        Console.Write("Enter Embed Title: ");
        string title = Console.ReadLine();
        Console.Write("Enter Embed Description: ");
        string description = Console.ReadLine();
        Console.Write("Enter Embed Color (Hex e.g., #FF5733): ");
        string color = Console.ReadLine();

        HttpClient client = new HttpClient();
        string jsonBody = $@"
{{
    ""embeds"": [
        {{
            ""title"": ""{title}"",
            ""description"": ""{description}"",
            ""color"": {ColorToHex(color)}
        }}
    ]
}}";

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        Console.WriteLine(response.IsSuccessStatusCode ?
            "Embed message sent successfully." :
            $"Failed. Status: {response.StatusCode}");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static string ColorToHex(string color)
    {
        return color.StartsWith("#") ? color.Replace("#", "") : color;
    }

    // 5. Message Scheduler
    static async Task MessageScheduler()
    {
        Console.Clear();
        Console.WriteLine("=== Message Scheduler ===");
        Console.Write("Enter Webhook URL: ");
        string url = Console.ReadLine();
        Console.Write("Enter message to send: ");
        string message = Console.ReadLine();
        Console.Write("Enter delay in seconds before sending message: ");
        int delay = int.Parse(Console.ReadLine());

        Console.WriteLine($"Scheduling message for {delay} seconds...");
        await Task.Delay(delay * 1000);

        HttpClient client = new HttpClient();
        var content = new StringContent($"{{\"content\":\"{message}\"}}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        Console.WriteLine(response.IsSuccessStatusCode ?
            "Message sent after delay." :
            $"Failed. Status: {response.StatusCode}");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 6. Fake Scanning Effect
    static async Task FakeScanningEffect()
    {
        Console.Clear();
        Console.WriteLine("Initializing Fake Scan...");
        string[] scanMessages = {
            "Scanning for vulnerabilities...",
            "Bypassing security...",
            "Injecting payload...",
            "Bypassing firewall...",
            "Disabling antivirus...",
            "Executing command..."
        };

        foreach (string message in scanMessages)
        {
            Console.Clear();
            Console.WriteLine($"[SCAN] {message}");
            await Task.Delay(1000);
        }
        Console.WriteLine("\nScan Complete. Press Enter to return...");
        Console.ReadLine();
    }

    // 7. Kick Member
    static async Task KickMember()
    {
        Console.Clear();
        Console.WriteLine("=== Kick Member ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();
        Console.Write("Enter User ID to kick: ");
        string userId = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.DeleteAsync($"https://discord.com/api/v9/guilds/{guildId}/members/{userId}");
        Console.WriteLine(response.IsSuccessStatusCode ?
            "User successfully kicked." :
            $"Failed to kick. Status: {response.StatusCode}");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 8. Ban Member
    static async Task BanMember()
    {
        Console.Clear();
        Console.WriteLine("=== Ban Member ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();
        Console.Write("Enter User ID to ban: ");
        string userId = Console.ReadLine();
        Console.Write("Enter Ban Reason (optional): ");
        string reason = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        string jsonBody = $"{{\"reason\": \"{reason}\"}}";
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"https://discord.com/api/v9/guilds/{guildId}/bans/{userId}", content);
        Console.WriteLine(response.IsSuccessStatusCode ?
            "User successfully banned." :
            $"Failed to ban. Status: {response.StatusCode}");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 9. Mass Role Assignment
    static async Task MassRoleAssignment()
    {
        Console.Clear();
        Console.WriteLine("=== Mass Role Assignment ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();
        Console.Write("Enter Role ID to assign: ");
        string roleId = Console.ReadLine();
        Console.Write("Enter User IDs to assign role (comma-separated): ");
        string[] userIds = Console.ReadLine().Split(',');

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");

        foreach (string userId in userIds.Select(u => u.Trim()))
        {
            var url = $"https://discord.com/api/v9/guilds/{guildId}/members/{userId}/roles/{roleId}";
            var response = await client.PutAsync(url, new StringContent(""));
            Console.WriteLine(response.IsSuccessStatusCode ?
                $"Role assigned to user {userId}." :
                $"Failed to assign role to user {userId}. Status: {response.StatusCode}");
            await Task.Delay(1000);
        }
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 10. Token Grabber
    static void TokenGrabber()
    {
        Console.Clear();
        Console.WriteLine("=== Token Grabber ===");
        Console.Write("Enter token to 'grab': ");
        string token = Console.ReadLine();
        Console.WriteLine($"Token captured: {token}");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 11. Mass DM Spammer
    static async Task MassDMSmammer()
    {
        Console.Clear();
        Console.WriteLine("=== Mass DM Spammer ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter comma-separated User IDs to DM: ");
        string[] userIds = Console.ReadLine().Split(',');
        Console.Write("Enter message to send: ");
        string message = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");

        foreach (string uid in userIds.Select(u => u.Trim()))
        {
            var dmContent = new StringContent($"{{\"recipient_id\":\"{uid}\"}}", Encoding.UTF8, "application/json");
            var dmResponse = await client.PostAsync("https://discord.com/api/v9/users/@me/channels", dmContent);
            if (dmResponse.IsSuccessStatusCode)
            {
                var dmChannelJson = await dmResponse.Content.ReadAsStringAsync();
                var channelId = ExtractChannelId(dmChannelJson);
                if (!string.IsNullOrEmpty(channelId))
                {
                    var msgContent = new StringContent($"{{\"content\":\"{message}\"}}", Encoding.UTF8, "application/json");
                    var msgResponse = await client.PostAsync($"https://discord.com/api/v9/channels/{channelId}/messages", msgContent);
                    Console.WriteLine(msgResponse.IsSuccessStatusCode ?
                        $"DM sent to user {uid}." :
                        $"Failed sending DM to {uid}. Status: {msgResponse.StatusCode}");
                }
            }
            else
            {
                Console.WriteLine($"Failed to create DM channel for user {uid}. Status: {dmResponse.StatusCode}");
            }
            await Task.Delay(1500);
        }
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static string ExtractChannelId(string json)
    {
        int idx = json.IndexOf("\"id\":\"");
        if (idx >= 0)
        {
            int start = idx + 6;
            int end = json.IndexOf("\"", start);
            if (end > start)
                return json.Substring(start, end - start);
        }
        return "";
    }

    // 12. Account Checker
    static void AccountChecker()
    {
        Console.Clear();
        Console.WriteLine("=== Account Checker ===");
        Console.Write("Enter the username to check: ");
        string username = Console.ReadLine();

        if (username.ToLower().Contains("taken"))
            Console.WriteLine($"Username '{username}' is NOT available.");
        else
            Console.WriteLine($"Username '{username}' appears available!");

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 13. Server Info Fetcher
    static async Task ServerInfoFetcher()
    {
        Console.Clear();
        Console.WriteLine("=== Server Info Fetcher ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}?with_counts=true");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Server Info:");
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine($"Failed to fetch server info. Status: {response.StatusCode}");
        }
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    // 14. Fake IP Address Generator
    static void FakeIPAddressGenerator()
    {
        Console.Clear();
        Console.WriteLine("=== Fake IP Address Generator ===");
        string fakeIP = GenerateFakeIP();
        Console.WriteLine($"Generated Fake IP: {fakeIP}");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static string GenerateFakeIP()
    {
        Random random = new Random();
        return $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}";
    }

    // 15. Password Strength Checker
    static void PasswordStrengthChecker()
    {
        Console.Clear();
        Console.WriteLine("=== Password Strength Checker ===");
        Console.Write("Enter a password to check: ");
        string password = Console.ReadLine();

        bool isStrong = CheckPasswordStrength(password);
        Console.WriteLine(isStrong ? "Password is strong." : "Password is weak.");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static bool CheckPasswordStrength(string password)
    {
        bool hasUpperChar = password.Any(c => Char.IsUpper(c));
        bool hasLowerChar = password.Any(c => Char.IsLower(c));
        bool hasDigit = password.Any(c => Char.IsDigit(c));
        bool hasSpecialChar = password.Any(c => !Char.IsLetterOrDigit(c));

        return hasUpperChar && hasLowerChar && hasDigit && hasSpecialChar && password.Length >= 8;
    }

    // ************* Nuker Menu & Nuker Functions ******************
    static async Task NukerMenu()
    {
        Console.Clear();
        // Change text color to green
        Console.ForegroundColor = ConsoleColor.Cyan;

        // Change background color to dark red
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("=== Nuker Menu ===");
        Console.WriteLine("WARNING: These actions are destructive and for testing only.");
        Console.WriteLine("[1] Channel Nuker");
        Console.WriteLine("[2] Role Nuker");
        Console.WriteLine("[3] Emoji Nuker");
        Console.WriteLine("[4] Spam Channel Creation");
        Console.WriteLine("[5] Combined Nuker Mode");
        Console.WriteLine("[6] Back to Main Menu");
        Console.Write("Select an option: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                await ChannelNuker();
                break;
            case "2":
                await RoleNuker();
                break;
            case "3":
                await EmojiNuker();
                break;
            case "4":
                await SpamChannelCreation();
                break;
            case "5":
                await CombinedNuker();
                break;
            case "6":
                return;
            default:
                Console.WriteLine("Invalid Option. Try Again.");
                break;
        }
        Console.WriteLine("Press Enter to return to Nuker Menu...");
        Console.ReadLine();
        await NukerMenu();
    }

    static async Task ChannelNuker()
    {
        Console.Clear();
        Console.WriteLine("=== Channel Nuker ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/channels");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch channels. Status: {response.StatusCode}");
            Console.ReadLine();
            return;
        }
        var channelsJson = await response.Content.ReadAsStringAsync();
        var channelIds = ExtractAllChannelIds(channelsJson);
        foreach (string channelId in channelIds)
        {
            var delResponse = await client.DeleteAsync($"https://discord.com/api/v9/channels/{channelId}");
            Console.WriteLine(delResponse.IsSuccessStatusCode ?
                $"Channel {channelId} deleted." :
                $"Failed to delete channel {channelId}. Status: {delResponse.StatusCode}");
            await Task.Delay(1000);
        }
        Console.WriteLine("Channel nuking complete.");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static async Task RoleNuker()
    {
        Console.Clear();
        Console.WriteLine("=== Role Nuker ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/roles");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch roles. Status: {response.StatusCode}");
            Console.ReadLine();
            return;
        }
        var rolesJson = await response.Content.ReadAsStringAsync();
        var roleIds = ExtractAllRoleIds(rolesJson);
        foreach (string roleId in roleIds)
        {
            if (roleId == guildId) continue;
            var delResponse = await client.DeleteAsync($"https://discord.com/api/v9/guilds/{guildId}/roles/{roleId}");
            Console.WriteLine(delResponse.IsSuccessStatusCode ?
                $"Role {roleId} deleted." :
                $"Failed to delete role {roleId}. Status: {delResponse.StatusCode}");
            await Task.Delay(1000);
        }
        Console.WriteLine("Role nuking complete.");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static async Task EmojiNuker()
    {
        Console.Clear();
        Console.WriteLine("=== Emoji Nuker ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/emojis");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch emojis. Status: {response.StatusCode}");
            Console.ReadLine();
            return;
        }
        var emojisJson = await response.Content.ReadAsStringAsync();
        var emojiIds = ExtractAllEmojiIds(emojisJson);
        foreach (string emojiId in emojiIds)
        {
            var delResponse = await client.DeleteAsync($"https://discord.com/api/v9/guilds/{guildId}/emojis/{emojiId}");
            Console.WriteLine(delResponse.IsSuccessStatusCode ?
                $"Emoji {emojiId} deleted." :
                $"Failed to delete emoji {emojiId}. Status: {delResponse.StatusCode}");
            await Task.Delay(1000);
        }
        Console.WriteLine("Emoji nuking complete.");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static async Task SpamChannelCreation()
    {
        Console.Clear();
        Console.WriteLine("=== Spam Channel Creation ===");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();
        Console.Write("Enter Channel Name Prefix: ");
        string channelPrefix = Console.ReadLine();
        Console.Write("Enter number of channels to create: ");
        int count = int.Parse(Console.ReadLine());

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        for (int i = 1; i <= count; i++)
        {
            string jsonBody = $"{{\"name\": \"{channelPrefix}-{i}\", \"type\": 0}}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://discord.com/api/v9/guilds/{guildId}/channels", content);
            Console.WriteLine(response.IsSuccessStatusCode ?
                $"Channel {channelPrefix}-{i} created." :
                $"Failed to create channel {channelPrefix}-{i}. Status: {response.StatusCode}");
            await Task.Delay(500);
        }
        Console.WriteLine("Channel creation complete.");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static async Task CombinedNuker()
    {
        Console.Clear();
        Console.WriteLine("=== Combined Nuker Mode ===");
        Console.WriteLine("WARNING: This mode will attempt to nuke channels, roles, and emojis.");
        Console.Write("Enter Bot Token: ");
        string botToken = Console.ReadLine();
        Console.Write("Enter Server (Guild) ID: ");
        string guildId = Console.ReadLine();

        await ChannelNuker_Internal(botToken, guildId);
        await RoleNuker_Internal(botToken, guildId);
        await EmojiNuker_Internal(botToken, guildId);

        Console.WriteLine("Combined nuker mode complete.");
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    static async Task ChannelNuker_Internal(string botToken, string guildId)
    {
        Console.WriteLine("Nuking channels...");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/channels");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch channels. Status: {response.StatusCode}");
            return;
        }
        var channelsJson = await response.Content.ReadAsStringAsync();
        var channelIds = ExtractAllChannelIds(channelsJson);
        foreach (string channelId in channelIds)
        {
            var delResponse = await client.DeleteAsync($"https://discord.com/api/v9/channels/{channelId}");
            Console.WriteLine(delResponse.IsSuccessStatusCode ?
                $"Channel {channelId} deleted." :
                $"Failed to delete channel {channelId}. Status: {delResponse.StatusCode}");
            await Task.Delay(1000);
        }
    }

    static async Task RoleNuker_Internal(string botToken, string guildId)
    {
        Console.WriteLine("Nuking roles...");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/roles");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch roles. Status: {response.StatusCode}");
            return;
        }
        var rolesJson = await response.Content.ReadAsStringAsync();
        var roleIds = ExtractAllRoleIds(rolesJson);
        foreach (string roleId in roleIds)
        {
            if (roleId == guildId) continue;
            var delResponse = await client.DeleteAsync($"https://discord.com/api/v9/guilds/{guildId}/roles/{roleId}");
            Console.WriteLine(delResponse.IsSuccessStatusCode ?
                $"Role {roleId} deleted." :
                $"Failed to delete role {roleId}. Status: {delResponse.StatusCode}");
            await Task.Delay(1000);
        }
    }

    static async Task EmojiNuker_Internal(string botToken, string guildId)
    {
        Console.WriteLine("Nuking emojis...");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {botToken}");
        var response = await client.GetAsync($"https://discord.com/api/v9/guilds/{guildId}/emojis");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch emojis. Status: {response.StatusCode}");
            return;
        }
        var emojisJson = await response.Content.ReadAsStringAsync();
        var emojiIds = ExtractAllEmojiIds(emojisJson);
        foreach (string emojiId in emojiIds)
        {
            var delResponse = await client.DeleteAsync($"https://discord.com/api/v9/guilds/{guildId}/emojis/{emojiId}");
            Console.WriteLine(delResponse.IsSuccessStatusCode ?
                $"Emoji {emojiId} deleted." :
                $"Failed to delete emoji {emojiId}. Status: {delResponse.StatusCode}");
            await Task.Delay(1000);
        }
    }

    static string[] ExtractAllChannelIds(string json)
    {
        var ids = new System.Collections.Generic.List<string>();
        int idx = 0;
        while ((idx = json.IndexOf("\"id\":\"", idx)) >= 0)
        {
            int start = idx + 6;
            int end = json.IndexOf("\"", start);
            if (end > start)
            {
                string id = json.Substring(start, end - start);
                ids.Add(id);
                idx = end;
            }
            else break;
        }
        return ids.ToArray();
    }

    static string[] ExtractAllRoleIds(string json) => ExtractAllChannelIds(json);
    static string[] ExtractAllEmojiIds(string json) => ExtractAllChannelIds(json);
}
