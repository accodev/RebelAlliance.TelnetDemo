// See https://aka.ms/new-console-template for more information

using RebelAlliance.TelnetDemo;

var server = new TelnetServer("0.0.0.0", 9999);
await server.Start();