using HtmlAgilityPack;
using System.Text.RegularExpressions;
using MD.PersianDateTime.Standard;
using MashrouhShared.Models;
using MashrouhShared.Helpers.Enums;

namespace MashrouhCataloger.Channels
{
    internal class IransedaChannel
    {
        public Channel Retrieve()
        {
            Channel iransedaChannel = new(
                type: ChannelType.Iranseda,
                url: new("https://radio.iranseda.ir/Program/?VALID=TRUE&ch=16&m=065139"));

            iransedaChannel.ReleaseDate = DateTime.UtcNow;

            HtmlWeb htmlWeb = new()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = System.Text.Encoding.UTF8
            };
            HtmlDocument htmlDoc = htmlWeb.Load(iransedaChannel.Url);
            HtmlNodeCollection htmlNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'episode-box')]//li");

            iransedaChannel.Sessions = new();

            foreach (HtmlNode li in htmlNode)
            {
                Session session = new();

                string? url = li.SelectSingleNode(".//a[@href]")?.GetAttributeValue("href", string.Empty).Trim();
                if (url != null && !string.IsNullOrEmpty(url))
                    session.Url = new(baseUri: iransedaChannel.Url, relativeUri: url);

                HtmlNode dateNode = li.SelectSingleNode(".//div[contains(@class,'dt-title')]");
                if (dateNode != null && PersianDateTime.TryParse(ParseDate(li.SelectSingleNode(".//div[contains(@class,'dt-title')]")), out PersianDateTime persianDateTimeResult))
                    session.Date = persianDateTimeResult;

                HtmlNode? descriptionNode = li.SelectSingleNode(".//div[contains(@class,'explain-box')]");
                if (descriptionNode != null)
                {
                    string? descriptionCandidate = Regex.Replace(descriptionNode.InnerText, @"\s+", " ").Trim();
                    if (descriptionNode != null && !string.IsNullOrEmpty(descriptionCandidate))
                        session.Description = descriptionCandidate;
                }

                iransedaChannel.Sessions.Add(session);
            };

            foreach (Session session in iransedaChannel.Sessions)
            {
                htmlDoc = htmlWeb.Load(session.Url);

                // #taglist2 .container>[class*=row-]>[class*=col-]:nth-child(2) article
                HtmlNodeCollection sectionsNode = htmlDoc.DocumentNode.SelectNodes("(//section[@id=\"taglist2\"]"
                + "//div[contains(@class,'container')]/div[contains(@class,'row-')]/div[contains(@class,'col-')])"
                + "[2]//article");

                if (sectionsNode == null || !sectionsNode.Any())
                    continue;

                List<SectionSummary> sectionSummaries = new();

                foreach (var articleNode in sectionsNode)
                    sectionSummaries.Add(ParseSectionSummary(articleNode));

                session.FileInfos = new();
                foreach (var summary in sectionSummaries)
                    session.FileInfos.Add(new()
                    {
                        Description = summary.Description,
                        FileUrlMirrors = ParseFileMirrorUrls(htmlDoc, summary.TargetModalId)
                    });
            }

            return iransedaChannel;
        }

        private string? ParseDate(HtmlNode dateNode)
        {
            if (dateNode == null)
                return null;

            HtmlNode dayNode = dateNode.SelectSingleNode(".//h2");
            if (dayNode == null)
                return null;
            string day = Regex.Replace(dayNode.InnerText, @"\s+", string.Empty);

            HtmlNode yearAndMonthNode = dateNode.SelectSingleNode(".//h4");
            if (yearAndMonthNode == null)
                return null;
            string yearAndMonth = Regex.Replace(yearAndMonthNode.InnerText, @"\s+", " ").Trim();

            return $"{day} {yearAndMonth}";
        }

        private record SectionSummary(string Description, string TargetModalId);

        private SectionSummary ParseSectionSummary(HtmlNode articleNode)
        {
            if (articleNode == null)
                throw new ArgumentNullException(nameof(articleNode));

            HtmlNode modalTriggerNode = articleNode.SelectSingleNode(".//a[contains(@data-toggle,'modal')][contains(@href,'#')]");
            if (modalTriggerNode == null)
                throw new Exception("Could not find modal trigger.");
            string modalId = modalTriggerNode.GetAttributeValue("href", string.Empty).Replace("#", string.Empty);

            string description = string.Empty;
            HtmlNode startTimeNode = articleNode.SelectSingleNode(".//span[contains(@class,'start_time')]");
            HtmlNode endTimeNode = articleNode.SelectSingleNode(".//span[contains(@class,'end_time')]");
            if (startTimeNode != null && endTimeNode != null)
                description += $"{startTimeNode.InnerText} - {endTimeNode.InnerText}";

            return new SectionSummary(description, modalId);
        }

        private List<Uri> ParseFileMirrorUrls(HtmlDocument htmlDoc, string modalId)
        {
            if (htmlDoc == null)
                throw new ArgumentNullException(nameof(htmlDoc));
            if (string.IsNullOrEmpty(modalId))
                throw new ArgumentNullException(nameof(modalId));

            HtmlNode modalNode = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id=\"{modalId}\"]");
            if (modalNode == null)
                throw new Exception("Could not find target modal.");

            HtmlNodeCollection mirrorAnchorNodes = modalNode.SelectNodes(".//a[@href]");
            if (!mirrorAnchorNodes.Any())
                return new();

            return mirrorAnchorNodes.Select(node => new Uri(node.GetAttributeValue("href", string.Empty))).ToList();
        }
    }
}
