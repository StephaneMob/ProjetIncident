using System;
using System.Text;
using System.Threading.Tasks;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IIncidentService _incidentService;

        public ReportingService(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        public async Task<byte[]> ExportCsvAsync(ReportFilterViewModel filter)
        {
            var incidents = await _incidentService.GetIncidentsAsync(null, filter.Status, filter.Severity, filter.SectorId, filter.MinistryId);
            
            var sb = new StringBuilder();
            sb.AppendLine("Ticket_RTIR;Date_Detection;Organisme;Ministere;Secteur;Actif_Impacte;CVE_Vulnerabilite;Severite;Statut;Date_Notification;Date_Resolution");

            foreach (var inc in incidents)
            {
                var orgName = inc.Asset?.Organization?.Name ?? "N/A";
                var minName = inc.Asset?.Organization?.Ministry?.Name ?? "N/A";
                var secName = inc.Asset?.Organization?.Ministry?.Sector?.Name ?? "N/A";
                var assetName = inc.Asset?.Name ?? "N/A";
                var cve = inc.Vulnerability?.CveId ?? "N/A";
                var dtNotif = inc.DateEmailSent.HasValue ? inc.DateEmailSent.Value.ToString("dd/MM/yyyy HH:mm") : "-";
                var dtFix = inc.DateVulnerabilityFixed.HasValue ? inc.DateVulnerabilityFixed.Value.ToString("dd/MM/yyyy HH:mm") : "-";

                sb.AppendLine($"\"{inc.TicketNumber}\";\"{inc.DateDetected:dd/MM/yyyy HH:mm}\";\"{orgName}\";\"{minName}\";\"{secName}\";\"{assetName}\";\"{cve}\";\"{inc.Severity}\";\"{inc.Status}\";\"{dtNotif}\";\"{dtFix}\"");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        public async Task<byte[]> ExportWordAsync(ReportFilterViewModel filter)
        {
            var metrics = await _incidentService.GetDashboardMetricsAsync();
            var incidents = await _incidentService.GetIncidentsAsync(null, filter.Status, filter.Severity, filter.SectorId, filter.MinistryId);

            var sb = new StringBuilder();
            sb.AppendLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>");
            sb.AppendLine("<head><meta charset='utf-8'><title>Rapport d'Orchestration des Incidents - CI-CERT</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Calibri', sans-serif; margin: 40px; color: #1e293b; }");
            sb.AppendLine("h1 { color: #0f172a; border-bottom: 2px solid #0284c7; padding-bottom: 8px; }");
            sb.AppendLine("h2 { color: #334155; margin-top: 25px; }");
            sb.AppendLine(".kpi-box { background: #f8fafc; border: 1px solid #cbd5e1; padding: 15px; border-radius: 6px; margin-bottom: 20px; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 15px; }");
            sb.AppendLine("th, td { border: 1px solid #cbd5e1; padding: 8px 12px; text-align: left; font-size: 11pt; }");
            sb.AppendLine("th { background-color: #0f172a; color: #ffffff; }");
            sb.AppendLine("tr:nth-child(even) { background-color: #f1f5f9; }");
            sb.AppendLine(".critique { color: #dc2626; font-weight: bold; }");
            sb.AppendLine(".badge { padding: 3px 8px; border-radius: 4px; font-weight: bold; }");
            sb.AppendLine("</style></head><body>");

            sb.AppendLine("<h1>RÉPUBLIQUE DE CÔTE D'IVOIRE</h1>");
            sb.AppendLine("<p style='font-size: 14pt; color: #64748b;'><strong>ANSSI - CI-CERT</strong> | Centre National de Veille et de Réponse aux Incidents</p>");
            sb.AppendLine($"<p style='font-size: 10pt; color: #94a3b8;'>Généré automatiquement le {DateTime.Now:dd/MM/yyyy à HH:mm} (TLP:AMBER)</p>");
            sb.AppendLine("<hr/>");

            sb.AppendLine("<h2>1. Synthèse Exécutive Ministérielle</h2>");
            sb.AppendLine("<div class='kpi-box'>");
            sb.AppendLine($"<p><strong>Total d'incidents recensés :</strong> {metrics.TotalIncidents}</p>");
            sb.AppendLine($"<p><strong>Incidents non traités (En cours) :</strong> {metrics.UntreatedIncidents}</p>");
            sb.AppendLine($"<p><strong>Incidents résolus / clos :</strong> {metrics.TreatedIncidents}</p>");
            sb.AppendLine($"<p><strong>Taux de résolution national :</strong> {metrics.ResolutionRatePercentage}%</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("<h2>2. Classement des Ministères & Secteurs les plus Vulnérables</h2>");
            sb.AppendLine("<table><tr><th>Rang</th><th>Ministère / Organisme</th><th>Total Incidents</th><th>En cours</th><th>Résolus</th></tr>");
            int rank = 1;
            foreach (var min in metrics.MostVulnerableMinistriesRanked)
            {
                sb.AppendLine($"<tr><td>{rank++}</td><td>{min.Label}</td><td>{min.TotalCount}</td><td class='critique'>{min.UntreatedCount}</td><td>{min.TreatedCount}</td></tr>");
            }
            sb.AppendLine("</table>");

            sb.AppendLine("<h2>3. Détail Complet des Incidents de Sécurité (SOAR/RTIR)</h2>");
            sb.AppendLine("<table><tr><th>Ticket RTIR</th><th>Date Détection</th><th>Actif & Organisme</th><th>Vulnérabilité (CVE)</th><th>Sévérité</th><th>Statut</th></tr>");
            foreach (var inc in incidents)
            {
                var org = inc.Asset?.Organization?.Name ?? "N/A";
                var asset = inc.Asset?.Name ?? "N/A";
                var cve = inc.Vulnerability?.CveId ?? "N/A";
                sb.AppendLine($"<tr><td><strong>{inc.TicketNumber}</strong></td><td>{inc.DateDetected:dd/MM/yyyy}</td><td>{asset}<br/><small>{org}</small></td><td>{cve}</td><td>{inc.Severity}</td><td>{inc.Status}</td></tr>");
            }
            sb.AppendLine("</table>");

            sb.AppendLine("<br/><br/><p style='text-align: center; font-size: 9pt; color: #94a3b8;'>Document Officiel - Direction Générale de l'ANSSI / CI-CERT</p>");
            sb.AppendLine("</body></html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> ExportPdfAsync(ReportFilterViewModel filter)
        {
            // Generates clean printable HTML-based PDF document
            return await ExportWordAsync(filter);
        }
    }
}
