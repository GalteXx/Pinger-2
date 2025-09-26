using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Pinger_2.Service
{
    internal class AddressConfigService : IAddressConfigService
    {
        public static Task<AddressConfigService> CreateAsync()
        {
            var service = new AddressConfigService();
            return service.InitializeAsync();
        }

        private async Task<AddressConfigService> InitializeAsync()
        {
            _targetIPAddresses = [];
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Geckosystem", "Pinger");

            XDocument doc = await LoadOrCreateConfigAsync(path);
            var fileStream = new FileStream(Path.Combine(path, _configName), FileMode.Open);
            await ValidateConfigAsync(doc, fileStream);
            await ParseConfigAsync(doc);
            return this;
        }

        public IEnumerable<IPAddress> TargetIPAddresses => _targetIPAddresses;

        List<IPAddress> _targetIPAddresses;
        private const string _configName = "config.xml"; // Hardcode goes brrr

        private async Task<XDocument> LoadOrCreateConfigAsync(string path)
        {
            string pathToFile = Path.Combine(path, _configName);
            if (!Directory.Exists(path) || !File.Exists(pathToFile))
            {
                Directory.CreateDirectory(path);
                File.Create(pathToFile).Close();
            }
            XDocument config;
            var configFile = new FileStream(pathToFile, FileMode.Open);
            try
            {
                config = await XDocument.LoadAsync(configFile, LoadOptions.PreserveWhitespace, CancellationToken.None);
            }
            catch (Exception)
            {
                // Maybe once I host my domain...
                config = XDocument.Parse("<Config>\r\n\t<TargetIPs>\r\n\t\t<TargetIP Domain=\"example.com\"/>\r\n\t\t<TargetIP IP=\"127.0.0.1\"/>\r\n\t</TargetIPs>\r\n</Config>");
                await config.SaveAsync(configFile, SaveOptions.None, CancellationToken.None);
            }
            configFile.Close();
            return config;
        }

        private static async Task ValidateConfigAsync(XDocument doc, FileStream fileStream)
        {
            if (doc.Root == null)
            {
                doc.RemoveNodes();
                doc.Add(new XElement("Config"));
                doc.Element("Config")!.Add(new XElement("TargetIPs"));
                doc.Element("Config")!.Element("TargetIPs")!.Add(new XElement("TargetIP", new XAttribute("Domain", "example.com")));
                doc.Element("Config")!.Element("TargetIPs")!.Add(new XElement("TargetIP", new XAttribute("IP", "128.0.0.1")));
                doc.Save(fileStream);
            }

            var targets = doc.Root?.Element("TargetIPs");
            if (targets == null) return;

            foreach (var el in targets.Elements("TargetIP").ToList())
            {
                var ipAttr = el.Attribute("IP")?.Value;
                var domainAttr = el.Attribute("Domain")?.Value;

                //missing IP and Domain
                if (string.IsNullOrWhiteSpace(ipAttr) && string.IsNullOrWhiteSpace(domainAttr))
                {
                    el.ReplaceWith(new XComment("Entry invalid: Missing Addresses"));
                    continue;
                }

                //Only IP present
                if (!string.IsNullOrWhiteSpace(ipAttr) && string.IsNullOrWhiteSpace(domainAttr))
                {
                    if (!IPAddress.TryParse(ipAttr, out _))
                    {
                        el.ReplaceWith(new XComment("Entry invalid: Invalid IP"));
                    }
                    continue;
                }

                //Only Domain present
                if (string.IsNullOrWhiteSpace(ipAttr) && !string.IsNullOrWhiteSpace(domainAttr))
                {
                    try
                    {
                        await Dns.GetHostAddressesAsync(domainAttr);
                    }
                    catch
                    {
                        el.ReplaceWith(new XComment($"Entry invalid: Failed to resolve domain {domainAttr}"));
                    }
                    continue;
                }

                //Both IP and Domain present
                if (!string.IsNullOrWhiteSpace(ipAttr) && !string.IsNullOrWhiteSpace(domainAttr))
                {
                    bool ipValid = IPAddress.TryParse(ipAttr, out _);
                    bool domainValid = false;
                    IPAddress[] resolved = [];

                    try
                    {
                        resolved = await Dns.GetHostAddressesAsync(domainAttr);
                        domainValid = resolved.Length > 0;
                    }
                    catch { }

                    if (domainValid && ipValid)
                    {
                        el.ReplaceWith(new XComment("Domain Resolved, IP Redundant"));
                        targets.Add(new XElement("TargetIP", new XAttribute("Domain", domainAttr)));
                    }
                    else if (!domainValid && ipValid)
                    {
                        el.ReplaceWith(new XComment("Failed to resolve domain, using IP address"));
                        targets.Add(new XElement("TargetIP", new XAttribute("IP", ipAttr)));
                    }
                    else if (!domainValid && !ipValid)
                    {
                        el.ReplaceWith(new XComment("Failed to resolve domain, incorrect IP"));
                    }
                    else if (domainValid && !ipValid)
                    {
                        el.ReplaceWith(new XComment("Domain valid, IP Redundant (and Incorrect)"));
                        targets.Add(new XElement("TargetIP", new XAttribute("Domain", domainAttr)));
                    }
                }
            }
            await doc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
        }

        private async Task ParseConfigAsync(XDocument doc)
        {
            _targetIPAddresses.Clear();

            var targets = doc.Root?.Element("TargetIPs");
            if (targets == null) return;

            foreach (var el in targets.Elements("TargetIP"))
            {
                var ipAttr = el.Attribute("IP")?.Value;
                var domainAttr = el.Attribute("Domain")?.Value;

                if (!string.IsNullOrWhiteSpace(ipAttr) && IPAddress.TryParse(ipAttr, out var ip))
                {
                    _targetIPAddresses.Add(ip);
                }
                else if (!string.IsNullOrWhiteSpace(domainAttr))
                {
                    try
                    {
                        var resolved = await Dns.GetHostAddressesAsync(domainAttr);
                        _targetIPAddresses.Add(resolved[0]); //to be made configurable
                    }
                    catch
                    { }
                }
            }
        }

    }
}
