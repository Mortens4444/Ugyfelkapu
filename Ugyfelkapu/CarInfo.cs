using Mtf.Utils.StringExtensions;
using System;
using System.Web;

namespace Ugyfelkapu
{
    class CarInfo
    {
        public string LicensePlate { get; private set; }

        public string Product { get; private set; }

        public string Type { get; private set; }

        public string Color { get; private set; }

        public bool Wanted { get; private set; }

        public string Insurance { get; private set; }

        public string TrafficData { get; private set; }

        public string Other { get; private set; }

        public CarInfo(string content)
        {
            ResponseChecker.CreckForError(content);

            LicensePlate = content.Substring("Rendszám: <strong>", "</strong>");
            Product = content.Substring("Gyártmány: <strong>", "</strong>");
            Type = content.Substring("Típus: <strong>", "</strong>");
            Color = HttpUtility.HtmlDecode(content.Substring("Szin: <strong>", "</strong>"));
            Wanted = !content.Substring("A jármű körözés alatt áll? <strong>", "</strong>").Equals("nem", StringComparison.CurrentCultureIgnoreCase);
            Insurance = HttpUtility.HtmlDecode(content.Substring("A kötelező gépjármű-felelősségbiztosítás adatai: <strong>", "</strong>"));
            TrafficData = HttpUtility.HtmlDecode(content.Substring("Forgalmi adatok: <strong>", "</strong>"));
            Other = HttpUtility.HtmlDecode(content.Substring("Egyéb: <strong>", "</strong>"));
        }

        public override string ToString()
        {
            return $"Rendszám: {LicensePlate}{Environment.NewLine}" +
                $"Gyártmány: {Product}{Environment.NewLine}" +
                $"Típus: {Type}{Environment.NewLine}" +
                $"Szín: {Color}{Environment.NewLine}" +
                (Wanted ? $"Körözés alatt áll.{Environment.NewLine}" : $"Nem áll körözés alatt.{Environment.NewLine}") +
                $"Kötelező gépjármű-felelősségbiztosítás adatai: {Insurance}{Environment.NewLine}" +
                $"Forgalmi adatok: {TrafficData}{Environment.NewLine}" +
                $"Egyéb: {Other}{Environment.NewLine}";
        }
    }
}
