using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Pinger_2.Service
{
    internal class AddressConfigService : IAddressConfigService
    {
        public IEnumerable<IPAddress> TargetIPAddresses => _targetIPAddresses;

        List<IPAddress> _targetIPAddresses;
        private const string _configName = "config.xml"; // Hardcode goes brrr

        public AddressConfigService()
        {
            _targetIPAddresses = [];
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Geckosystem", "Pinger");

            XDocument doc = LoadOrCreateConfig(path);
            ValidateConfig(doc, Path.Combine(path, _configName));
        }

        private XDocument LoadOrCreateConfig(string path)
        {
            string pathToFile = Path.Combine(path, _configName);
            if (!Directory.Exists(path) || !File.Exists(pathToFile))
            {
                Directory.CreateDirectory(path);
                File.Create(pathToFile).Close();
            }
            XDocument config;
            try
            {
                config = XDocument.Load(pathToFile);
            }
            catch (Exception)
            {
                // Maybe once I host my domain...
                config = XDocument.Parse("<Config>\r\n\t<TargetIPs>\r\n\t\t<TargetIP Domain=\"example.com\"/>\r\n\t\t<TargetIP IP=\"127.0.0.1\"/>\r\n\t</TargetIPs>\r\n</Config>");
                config.Save(pathToFile);
            }
            return config;
        }

        private static void ValidateConfig(XDocument doc, string pathToConfigFile)
        {
            if (doc.Root == null)
            {
                doc.RemoveNodes();
                doc.Add(new XElement("Config"));
                doc.Element("Config")!.Add(new XElement("TargetIPs"));
                doc.Element("Config")!.Element("TargetIPs")!.Add(new XElement("TargetIP", new XAttribute("Domain", "example.com")));
                doc.Element("Config")!.Element("TargetIPs")!.Add(new XElement("TargetIP", new XAttribute("IP", "128.0.0.1")));
                doc.Save(pathToConfigFile);
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
                        Dns.GetHostAddresses(domainAttr);
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
                        resolved = Dns.GetHostAddresses(domainAttr);
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
                        el.ReplaceWith(new XComment("Domain valid, IP invalid → using Domain only"));
                        targets.Add(new XElement("TargetIP", new XAttribute("Domain", domainAttr)));
                    }
                }
            }
            doc.Save(pathToConfigFile);
        }
        
    }
}
