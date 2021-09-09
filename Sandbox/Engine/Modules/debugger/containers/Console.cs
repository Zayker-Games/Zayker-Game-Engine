using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ZEngine.Debugging
{
    class Console : Container
    {
        public enum LogLevel
        {
            message,
            warning,
            error
        }

        public LogLevel currentVisibilityLevel = LogLevel.message;
        public List<ConsoleMessage> messages = new List<ConsoleMessage>();

        bool scrollToBottom = false;

        public Console (DebuggerGuiInstance debugger)
        {
            base.Init(debugger);
            name = "Console";
            opened = true;
        }

        public override void Update(float dt)
        {
            // Clamp log to 1000 messages
            while (messages.Count > 1000)
            {
                messages.RemoveAt(0);
            }

            // Draw console
            if (opened)
            {
                // Set up window
                ImGui.SetNextWindowSizeConstraints(new Vector2(500, 500), new Vector2(2000, 2000));
                ImGui.Begin("Console##" + id, ref opened);

                // Header Bar
                if (ImGui.Button("Clear")) { messages.Clear(); }
                ImGui.SameLine();

                // Get the name of the current visibility level (stupid code -> replace!)
                string currentVisibilityLevelString = currentVisibilityLevel.ToString() + "s";
                currentVisibilityLevelString = currentVisibilityLevelString.Insert(0, currentVisibilityLevelString[0].ToString().ToUpper());
                currentVisibilityLevelString = currentVisibilityLevelString.Remove(1, 1);

                // Show visibility-level selector
                if (ImGui.BeginCombo("##" + id, currentVisibilityLevelString))
                {
                    if (ImGui.Selectable("Messages")) { currentVisibilityLevel = LogLevel.message; }
                    if (ImGui.Selectable("Warnings")) { currentVisibilityLevel = LogLevel.warning; }
                    if (ImGui.Selectable("Errors")) { currentVisibilityLevel = LogLevel.error; }
                    ImGui.EndCombo();
                }

                ImGui.Separator();

                // Display messages
                ImGui.BeginChild("scrolling", ImGui.GetWindowSize() - new Vector2(10f, 90f));
                foreach (ConsoleMessage message in messages)
                {
                    if (message.logLevel >= currentVisibilityLevel)
                    {
                        Vector4 color = new Vector4(1f, 1f, 1f, 1f);
                        if (message.logLevel == LogLevel.warning)
                            color = new Vector4(1f, 0.6f, 0.0f, 1f);
                        if (message.logLevel == LogLevel.error)
                            color = new Vector4(1f, 0f, 0f, 1f);

                        ImGui.TextColored(color, message.message);
                        ImGui.TextWrapped(message.description);
                        ImGui.Separator();
                    }
                }

                // Scroll all the way to the bottom if prompted
                if (scrollToBottom)
                {
                    ImGui.SetScrollY(100000.0f);
                    scrollToBottom = false;
                }

                ImGui.EndChild();
                ImGui.Separator();

                // Handle input field
                ImGuiInputTextFlags input_text_flags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory;
                byte[] inputBuffer = new byte[100];
                if(ImGui.InputText("", inputBuffer, (uint)Buffer.ByteLength(inputBuffer), input_text_flags))
                {
                    HandleCommand(Encoding.Default.GetString(inputBuffer));
                }

                ImGui.End();
            }
        }

        public void WriteToConsole(string message, string description, LogLevel logLevel = LogLevel.message)
        {
            messages.Add(new ConsoleMessage(message, description, logLevel));
            scrollToBottom = true;

            // Errors are also logged to the system console
            if (logLevel == LogLevel.error)
                System.Console.WriteLine("[ERROR]: " + message);
        }

        public void HandleCommand(string input)
        {
            // The following code will be replaced by a system allowing for custom commands

            // Remove \0 characters created by input field
            input = input.Trim(new char[] { ' ', '\0' });

            // Split input into command and arguments
            string command = "";
            string[] args = new string[0];
            if (input.Split(" ").Length > 1)
            {
                command = input.Split(" ")[0];
                args = input.Substring(input.IndexOf(" ")).Split(" ", StringSplitOptions.RemoveEmptyEntries);
            } else
            {
                command = input;
                args = new string[0];
            }

            // Execute commands
            if (command == "message" || command == "print" || command == "log")
                WriteToConsole(String.Join(" ", args), "Message send using log command.", LogLevel.message);

            if (command == "warning" || command == "warn")
                WriteToConsole(String.Join(" ", args), "Warning send using log command.", LogLevel.warning);

            if (command == "error")
                WriteToConsole(String.Join(" ", args), "Error send using log command.", LogLevel.error);

            if (command == "cls" || command == "clear")
                messages.Clear();

            if(command == "help")
                WriteToConsole("Help", "This is still work in progress. ", LogLevel.message);
        }

        public class ConsoleMessage
        {
            public string message;
            public string description;
            public LogLevel logLevel;

            public ConsoleMessage(string message, string description, LogLevel logLevel = LogLevel.message)
            {
                this.message = message;
                this.description = description;
                this.logLevel = logLevel;
            }
        }
    }
}
