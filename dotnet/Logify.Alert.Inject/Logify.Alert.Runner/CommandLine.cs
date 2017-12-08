using System;
using System.Collections.Generic;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    public class CommandLineParser {
        CommandLineSwitch CurrentSwitch { get; set; }
        List<string> UnclaimedValues = new List<string>();
        int UnclaimedFirstIndex { get; set; }

        public Dictionary<string, CommandLineSwitch> Parse(string[] args, CommandLineConfiguration configuration) {
            StringComparer comparer = configuration != null && configuration.IsCaseSensitive ? StringComparer.InvariantCulture: StringComparer.InvariantCultureIgnoreCase;
            Dictionary <string, CommandLineSwitch> result = new Dictionary<string, CommandLineSwitch>(comparer);
            if (args == null || args.Length <= 0 || configuration == null/* || configuration.Count <= 0*/)
                return result;


            this.CurrentSwitch = null;
           
            int count = args.Length;
            for (int i = 0; i < count; i++) {
                CommandLineSwitch @switch = TryParseSwitch(args[i], configuration);
                if (@switch != null)
                    AppendSwitch(@switch, result, i);
                else
                    AppendValue(args[i], result, i);
            }

            if (UnclaimedValues.Count > 0) {
                CommandLineSwitch defaultOption = DetectDefaultOptionName(configuration);
                if (defaultOption != null) {
                    CommandLineSwitch item = defaultOption.Clone();
                    item.Values = UnclaimedValues;
                    item.Index = UnclaimedFirstIndex;
                    result[defaultOption.Name] = item;
                    //result[defaultOption.Name] = unclaimedValues;
                }
            }

            return result;
        }

        CommandLineSwitch TryParseSwitch(string text, CommandLineConfiguration configuration) {
            if (String.IsNullOrEmpty(text))
                return null;

            if (text.StartsWith("--"))
                return ParseSwitchCore(text.Substring(2), configuration);
            else if (text[0] == '/' || text[0] == '-')
                return ParseSwitchCore(text.Substring(1), configuration);
            else
                return null;
        }
        CommandLineSwitch ParseSwitchCore(string text, CommandLineConfiguration configuration) {
            int count = text.Length;
            for (int i = 0; i < count; i++) {
                if (text[i] == '=' || text[i] == ':') {
                    return CreateSwitch(text.Substring(0, i).Trim(), text.Substring(i + 1).Trim(), configuration);
                }
            }
            return CreateSwitch(text.Trim(), null, configuration);
        }

        CommandLineSwitch CreateSwitch(string name, string value, CommandLineConfiguration configuration) {
            CommandLineSwitch result = FindSwitch(configuration, name);
            if (result != null)
                result = result.Clone();
            else {
                result = new CommandLineSwitch();
                result.Name = name;
            }
            result.Value = value;
            return result;
        }

        void AppendValue(string value, Dictionary<string, CommandLineSwitch> result, int index) {
            if (this.CurrentSwitch == null) {
                if (this.UnclaimedValues.Count <= 0)
                    this.UnclaimedFirstIndex = index;
                this.UnclaimedValues.Add(value);
            }
            else
                AppendValueToSwitch(result, this.CurrentSwitch, value);
        }

        void AppendSwitch(CommandLineSwitch @switch, Dictionary<string, CommandLineSwitch> result, int index) {
            CommandLineSwitch existingSwitch;
            if (!result.TryGetValue(@switch.Name, out existingSwitch)) {
                this.CurrentSwitch = @switch.Clone();
                this.CurrentSwitch.Index = index;
            }
            else {
                if (existingSwitch.IsMultiple) {
                    this.CurrentSwitch = existingSwitch;
                }
                else {
                    this.CurrentSwitch = @switch.Clone();
                    this.CurrentSwitch.Index = index;
                }
            }

            AppendValueToSwitch(result, this.CurrentSwitch, @switch.Value);
            if (!@switch.ExpectValue)
                this.CurrentSwitch = null;
        }

        void AppendValueToSwitch(Dictionary<string, CommandLineSwitch> result, CommandLineSwitch @switch, string value) {
            if (@switch.IsMultiple) {
                //List<object> values;
                List<string> values;
                if (!result.ContainsKey(@switch.Name)) {
                    values = new List<string>();
                    @switch.Value = null;
                    @switch.Values = values;
                    //values = new List<object>();
                    //result[@switch.Name] = values;
                    result[@switch.Name] = @switch;
                }
                else {
                    values = result[@switch.Name].Values;
                    //values = (List<object>)result[@switch.Name];
                }
                if (!String.IsNullOrEmpty(value))
                    values.Add(value);
            }
            else {
                @switch.Value = value;
                result[@switch.Name] = @switch;
                if (!String.IsNullOrEmpty(value))
                    this.CurrentSwitch = null;
            }
            //result[@switch.Name] = value;
        }

        CommandLineSwitch DetectDefaultOptionName(CommandLineConfiguration configuration) {
            foreach (CommandLineSwitch item in configuration)
                if (item.IsDefaultOption)
                    return item;

            return null;
        }
        CommandLineSwitch FindSwitch(CommandLineConfiguration configuration, string name) {
            foreach (CommandLineSwitch item in configuration)
                if (String.Compare(item.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0 ||
                    String.Compare(item.Alias, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return item;

            return null;
        }

        public string GetUsage(string execName, CommandLineConfiguration configuration, int maxLineSize = 80) {
            StringBuilder result = new StringBuilder();
            result.Append(execName);
            result.Append(' ');
            int count = configuration.Count;
            for (int i = 0; i < count; i++) {
                result.Append(GetArgUsage(configuration[i]));
                result.Append(' ');
            }
            return result.ToString();
        }

        string GetArgUsage(CommandLineSwitch commandLineSwitch) {
            string name = commandLineSwitch.Name;
            if (commandLineSwitch.ExpectValue) {
                if (commandLineSwitch.IsDefaultOption)
                    name = "<" + name + ">";
                if (commandLineSwitch.IsMultiple)
                    return String.Format("[--{0}=<value1>, ...]", name);
                else
                    return String.Format("[--{0}=<value>]", name);
            }
            else
                return String.Format("[--{0}]", name);
        }

        public string GetParametersDescription(CommandLineConfiguration configuration, int maxLineSize = 80) {
            List<string> switches = new List<string>();
            List<string> descriptions = new List<string>();
            int count = configuration.Count;
            int maxSwitchSize = -1;
            for (int i = 0; i < count; i++) {
                string text = GetSwitchesString(configuration[i]);
                maxSwitchSize = Math.Max(text.Length, maxSwitchSize);
                switches.Add(text);
                descriptions.Add(configuration[i].Description);
            }

            maxSwitchSize += 2;

            int descriptionSize = maxLineSize - maxSwitchSize;
            if (descriptionSize < 20)
                descriptionSize = 80;


            StringBuilder result = new StringBuilder();
            for (int i = 0; i < count; i++) {
                result.Append(FormatSwitches(switches[i], maxSwitchSize));
                result.Append(FormatDescription(descriptions[i], maxSwitchSize, descriptionSize));
            }
            return result.ToString();
        }

        string FormatSwitches(string text, int maxSwitchSize) {
            if (text.Length < maxSwitchSize)
                text += new string(' ', maxSwitchSize - text.Length);
            return text;
        }
        string FormatDescription(string text, int maxSwitchSize, int descriptionMaxSize) {
            string leftPadding = new string(' ', maxSwitchSize);
            StringBuilder result = new StringBuilder();
            StringBuilder line = new StringBuilder();
            int currentLineSize = 0;

            string[] words = text.Split(' ');
            int count = words.Length;
            for (int i = 0; i < count; i++) {
                int itemSize = words[i].Length + 1;
                if (currentLineSize + itemSize > descriptionMaxSize) {
                    if (line.Length <= 0) {
                        AppendDescriptionLine(result, words[i], leftPadding);
                    }
                    else {
                        AppendDescriptionLine(result, line.ToString(), leftPadding);
                        currentLineSize = itemSize;
                        line.Length = 0;
                        line.Append(words[i]);
                        line.Append(' ');
                    }
                }
                else {
                    currentLineSize += itemSize;
                    line.Append(words[i]);
                    line.Append(' ');
                }
            }

            if (line.Length > 0)
                AppendDescriptionLine(result, line.ToString(), leftPadding);
            return result.ToString();
        }
        void AppendDescriptionLine(StringBuilder result, string text, string leftPadding) {
            if (result.Length > 0)
                result.Append(leftPadding);
            result.AppendLine(text);
        }
        string GetSwitchesString(CommandLineSwitch sw) {
            string result = "--" + sw.Name;
            if (!String.IsNullOrEmpty(sw.Alias))
                result += ", -" + sw.Alias;
            return result;
        }
    }

    public class CommandLineConfiguration : List<CommandLineSwitch> {
        public bool IsCaseSensitive { get; set; }
    }

    public class CommandLineSwitch {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }

        //public Type Type { get; set; } // null or boolean to binary switch
        public bool IsMultiple { get; set; }
        public object DefaultValue { get; set; }
        public bool IsDefaultOption { get; set; }
        public bool ExpectValue { get; set; }

        internal int Index { get; set; }
        internal List<string> Values { get; set; }
        internal string Value { get; set; }

        public CommandLineSwitch() {
            this.Index = -1;
        }

        public CommandLineSwitch Clone() {
            CommandLineSwitch clone = new CommandLineSwitch();
            clone.CopyFrom(this);
            return clone;
        }

        void CopyFrom(CommandLineSwitch other) {
            this.Name = other.Name;
            this.Alias = other.Alias;
            this.Description = other.Description;
            //this.Type  = other.Type;
            this.IsMultiple = other.IsMultiple;
            this.DefaultValue = other.DefaultValue;
            this.IsDefaultOption = other.IsDefaultOption;
            this.ExpectValue = other.ExpectValue;
            this.Value = other.Value;
            this.Index = other.Index;
        }
    }
}