using System;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
    [TestFixture]
    public class CommandLineParserTests {
        CommandLineParser parser;
        [SetUp]
        public void Setup() {
            this.parser = new CommandLineParser();
        }
        [TearDown]
        public void TearDown() {
            this.parser = null;
        }

        [Test]
        public void NullArgsNullConfig() {
            var result = parser.Parse(null, null);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void EmptyArgsNullConfig() {
            var result = parser.Parse(new string[] { }, null);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void NullConfig() {
            var result = parser.Parse(new string[] { "--test" }, null);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void NullArgs() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            var result = parser.Parse(null, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void EmptyArgs() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            var result = parser.Parse(new string[] { }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public void EmptyConfig() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            var result = parser.Parse(new string[] { "--test" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SimpleSwitchDoubleDash() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
            });
            var result = parser.Parse(new string[] { "--test" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SimpleSwitchSingleDash() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
            });
            var result = parser.Parse(new string[] { "-test" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SimpleSwitchSlash() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
            });
            var result = parser.Parse(new string[] { "/test" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SimpleSwitchAliasDoubleDash() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
                Alias = "t"
            });
            var result = parser.Parse(new string[] { "--t" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SimpleSwitchAliasSingleDash() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
                Alias = "t"
            });
            var result = parser.Parse(new string[] { "-t" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SimpleSwitchAliasSlash() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
                Alias = "t"
            });
            var result = parser.Parse(new string[] { "/t" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SwitchWithValueSeparatorEquals() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
            });
            var result = parser.Parse(new string[] { "-test=1" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual("1", item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SwitchWithValueSeparatorColon() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
            });
            var result = parser.Parse(new string[] { "-test:1" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual("1", item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void SwitchWithValueSeparatorSpace() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
                ExpectValue = true,
            });
            var result = parser.Parse(new string[] { "-test", "1" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual("1", item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void OverwriteValue() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "test",
                ExpectValue = true,
            });
            var result = parser.Parse(new string[] { "-test", "1" , "/test=2" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("test"));
            CommandLineSwitch item = result["test"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(2, item.Index);
            Assert.AreEqual("2", item.Value);
            Assert.AreEqual(null, item.Values);
        }
        [Test]
        public void MultiValueSwitchWithSwitches() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "file",
                IsMultiple = true,
                ExpectValue = true,
            });
            var result = parser.Parse(new string[] { "-file", "f1", "/file=f2", "/file:f3" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("file"));
            CommandLineSwitch item = result["file"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(true, item.Values != null);
            Assert.AreEqual(3, item.Values.Count);
            Assert.AreEqual("f1", item.Values[0]);
            Assert.AreEqual("f2", item.Values[1]);
            Assert.AreEqual("f3", item.Values[2]);
        }
        [Test]
        public void MultipleUnclaimed() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "file",
                IsMultiple = true,
                IsDefaultOption = true
            });
            var result = parser.Parse(new string[] { "f1", "f2", "f3" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(true, result.ContainsKey("file"));
            CommandLineSwitch item = result["file"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(true, item.Values != null);
            Assert.AreEqual(3, item.Values.Count);
            Assert.AreEqual("f1", item.Values[0]);
            Assert.AreEqual("f2", item.Values[1]);
            Assert.AreEqual("f3", item.Values[2]);
        }
        [Test]
        public void SimpleUnclaimedOption() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "win",
            });
            config.Add(new CommandLineSwitch() {
                Name = "exec",
                IsMultiple = true,
                IsDefaultOption = true,
                ExpectValue = true,
            });
            var result = parser.Parse(new string[] { "--win", "C:\\temp\\inject.log" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(true, result.ContainsKey("win"));
            CommandLineSwitch item = result["win"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);

            Assert.AreEqual(true, result.ContainsKey("exec"));
            item = result["exec"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(1, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(true, item.Values != null);
            Assert.AreEqual(1, item.Values.Count);
            Assert.AreEqual("C:\\temp\\inject.log", item.Values[0]);
        }
        [Test]
        public void CommandLineInsideCommandLine() {
            CommandLineConfiguration config = new CommandLineConfiguration();
            config.Add(new CommandLineSwitch() {
                Name = "exec",
                IsMultiple = true,
                IsDefaultOption = true
            });
            var result = parser.Parse(new string[] { "--win", "--log=C:\\temp\\inject.log", "executable.exe", "--param1", "/param2" }, config);
            Assert.AreEqual(true, result != null);
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(true, result.ContainsKey("win"));
            CommandLineSwitch item = result["win"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);

            Assert.AreEqual(true, result.ContainsKey("log"));
            item = result["log"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(1, item.Index);
            Assert.AreEqual(@"C:\temp\inject.log", item.Value);
            Assert.AreEqual(null, item.Values);

            Assert.AreEqual(true, result.ContainsKey("exec"));
            item = result["exec"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(2, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(true, item.Values != null);
            Assert.AreEqual(1, item.Values.Count);
            Assert.AreEqual("executable.exe", item.Values[0]);

            Assert.AreEqual(true, result.ContainsKey("param1"));
            item = result["param1"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(3, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);

            Assert.AreEqual(true, result.ContainsKey("param2"));
            item = result["param2"];
            Assert.AreEqual(true, item != null);
            Assert.AreEqual(4, item.Index);
            Assert.AreEqual(null, item.Value);
            Assert.AreEqual(null, item.Values);
        }
    }
}