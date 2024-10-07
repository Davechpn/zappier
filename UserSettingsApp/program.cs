using System;
using System.IO;

namespace UserSettingsApp
{
    class Program
    {
        // Function to save the user settings as a byte in a file
        public static void SaveSettingsToFile(string binarySettings, string filePath)
        {
            // Ensure the binary string is exactly 8 bits long before converting
            binarySettings = binarySettings.PadLeft(8, '0');
            // Convert the binary string to a byte for storage
            byte settingsByte = Convert.ToByte(binarySettings, 2);
            // Write the byte to the specified file
            File.WriteAllBytes(filePath, new byte[] { settingsByte });
        }

        // Function to load the user settings from a file and convert them to a binary string
        public static string LoadSettingsFromFile(string filePath)
        {
            // Read the byte from the file
            byte[] settingsByte = File.ReadAllBytes(filePath);
            // Convert the byte back to an 8-bit binary string, padded with leading zeros
            return Convert.ToString(settingsByte[0], 2).PadLeft(8, '0');
        }

        // Function to check if a specific feature is enabled based on the binary settings
        public static bool IsFeatureEnabled(string binarySettings, int featureIndex)
        {
            // Reverse the binary string to match left-to-right ordering
            string reversedBinarySettings = ReverseString(binarySettings);

            // Convert the reversed binary string to an integer for bitwise operations
            int settingsValue = Convert.ToInt32(reversedBinarySettings, 2);

            // Check if the bit at the specified index is set to 1 (enabled)
            return (settingsValue & (1 << featureIndex)) != 0;
        }

        // Helper function to reverse the binary string
        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        // Function to get initial settings from the user by asking them to enable/disable features by name
        public static string GetInitialSettings(string[] featureNames)
        {
            string binarySettings = "";

            // Loop through each feature and ask the user to enable or disable it by feature name with number
            for (int i = 0; i < featureNames.Length; i++)
            {
                Console.Write($"Enable {i + 1} - {featureNames[i]}? (1 for Yes, 0 for No): ");
                string input;

                // Ensure the input is valid (either 0 or 1)
                while (!((input = Console.ReadLine()) == "0" || input == "1"))
                {
                    Console.WriteLine("Invalid input. Please enter 1 (Yes) or 0 (No).");
                }

                // Append the input to the binary settings string
                binarySettings += input;
            }

            return binarySettings;
        }

        static void Main(string[] args)
        {
            string filePath = "user_settings.bin";

            // Array of feature names
            string[] featureNames = {
                "SMS Notifications",
                "Push Notifications",
                "Bio-metrics",
                "Camera",
                "Location",
                "NFC",
                "Vouchers",
                "Loyalty"
            };

            // Get initial settings from the user by asking about each feature by name
            string userSettings = GetInitialSettings(featureNames);

            // Save the binary settings to a file
            SaveSettingsToFile(userSettings, filePath);
            Console.WriteLine("User settings have been saved to the file.");

            // Load the binary settings from the file
            string loadedSettings = LoadSettingsFromFile(filePath);
            Console.WriteLine($"User settings loaded from file: {loadedSettings}");

            // Loop to allow checking multiple features until the user decides to exit
            while (true)
            {
                // Prompt the user to enter the feature number (1-8) they want to check
                Console.WriteLine("Enter the feature number you want to check (1-8), or enter 0 to exit:");

                int featureNumber;

                // Ensure the input is valid (a number between 1 and 8, or 0 to exit)
                while (!int.TryParse(Console.ReadLine(), out featureNumber) || featureNumber < 0 || featureNumber > 8)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 0 and 8.");
                }

                // Exit if the user enters 0
                if (featureNumber == 0)
                {
                    Console.WriteLine("Exiting feature check.");
                    break;
                }

                // Convert the feature number to the corresponding index (0-7)
                int featureIndex = featureNumber - 1;

                // Determine if the selected feature is enabled and display the result
                bool isFeatureEnabled = IsFeatureEnabled(loadedSettings, featureIndex);
                Console.WriteLine(isFeatureEnabled
                                  ? $"{featureNumber} - {featureNames[featureIndex]} is enabled."
                                  : $"{featureNumber} - {featureNames[featureIndex]} is disabled.");
            }
        }
    }
}
