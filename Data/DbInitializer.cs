using System;
using System.Linq;
using CICertSOAR.Models;

namespace CICertSOAR.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Sectors.Any())
            {
                return; // DB already seeded
            }

            // 1. Sectors
            var publicSector = new Sector { Name = "Public & Administration" };
            var financeSector = new Sector { Name = "Finance & Économie" };
            var telecomSector = new Sector { Name = "Télécoms & Numérique" };
            var energySector = new Sector { Name = "Énergie & Mines" };
            var healthSector = new Sector { Name = "Santé & Protection Sociale" };

            context.Sectors.AddRange(publicSector, financeSector, telecomSector, energySector, healthSector);
            context.SaveChanges();

            // 2. Ministries
            var mntd = new Ministry { Name = "Ministère de la Transition Numérique et de la Digitalisation", SectorId = telecomSector.Id };
            var mef = new Ministry { Name = "Ministère de l'Économie, des Finances et du Budget", SectorId = financeSector.Id };
            var mis = new Ministry { Name = "Ministère de l'Intérieur et de la Sécurité", SectorId = publicSector.Id };
            var msh = new Ministry { Name = "Ministère de la Santé et de l'Hygiène Publique", SectorId = healthSector.Id };
            var mmpe = new Ministry { Name = "Ministère des Mines, du Pétrole et de l'Énergie", SectorId = energySector.Id };

            context.Ministries.AddRange(mntd, mef, mis, msh, mmpe);
            context.SaveChanges();

            // 3. Organizations
            var anssi = new Organization 
            { 
                Name = "ANSSI - Agence Nationale de la Sécurité des Systèmes d'Information", 
                MinistryId = mntd.Id, 
                ContactName = "Kouassi Jean-Paul (CERT Head)", 
                ContactEmail = "cert-contact@anssi.gouv.ci", 
                ContactPhone = "+225 07 08 09 10 11" 
            };
            var ansut = new Organization 
            { 
                Name = "ANSUT - Agence Nationale du Service Universel des Télécommunications", 
                MinistryId = mntd.Id, 
                ContactName = "Yao Patricia (RSSI)", 
                ContactEmail = "rssi@ansut.ci", 
                ContactPhone = "+225 01 02 03 04 05" 
            };
            var artci = new Organization 
            { 
                Name = "ARTCI - Autorité de Régulation des Télécommunications/TIC", 
                MinistryId = mntd.Id, 
                ContactName = "Diomandé Bakary (Soc Lead)", 
                ContactEmail = "soc@artci.ci", 
                ContactPhone = "+225 05 06 07 08 09" 
            };
            var dgi = new Organization 
            { 
                Name = "DGI - Direction Générale des Impôts", 
                MinistryId = mef.Id, 
                ContactName = "Kone Ibrahim (DSI Impôts)", 
                ContactEmail = "dsi-securite@impots.gouv.ci", 
                ContactPhone = "+225 07 44 55 66 77" 
            };
            var tresor = new Organization 
            { 
                Name = "Direction Générale du Trésor et de la Comptabilité Publique", 
                MinistryId = mef.Id, 
                ContactName = "Bamba Sekou (RSSI Trésor)", 
                ContactEmail = "cybersecurite@tresor.gouv.ci", 
                ContactPhone = "+225 01 99 88 77 66" 
            };
            var chu = new Organization 
            { 
                Name = "CHU d'Angré / Plateforme Hospitalière", 
                MinistryId = msh.Id, 
                ContactName = "Dr. N'Guessan Charles", 
                ContactEmail = "it-support@chu-angre.ci", 
                ContactPhone = "+225 07 11 22 33 44" 
            };

            context.Organizations.AddRange(anssi, ansut, artci, dgi, tresor, chu);
            context.SaveChanges();

            // 4. Assets
            var asset1 = new Asset 
            { 
                Name = "Portail Officiel du Gouvernement", 
                IpAddress = "160.155.12.10", 
                Domain = "gouv.ci", 
                Type = "Web", 
                Criticality = "Critique", 
                OrganizationId = anssi.Id, 
                DateRegistered = DateTime.Now.AddMonths(-6) 
            };
            var asset2 = new Asset 
            { 
                Name = "Passerelle Fiscale Télédéclaration e-Impots", 
                IpAddress = "160.155.45.22", 
                Domain = "e-impots.gouv.ci", 
                Type = "API", 
                Criticality = "Critique", 
                OrganizationId = dgi.Id, 
                DateRegistered = DateTime.Now.AddMonths(-8) 
            };
            var asset3 = new Asset 
            { 
                Name = "Système Central de Paiement TrésorPay", 
                IpAddress = "160.155.88.5", 
                Domain = "tresorpay.gouv.ci", 
                Type = "Web", 
                Criticality = "Critique", 
                OrganizationId = tresor.Id, 
                DateRegistered = DateTime.Now.AddMonths(-12) 
            };
            var asset4 = new Asset 
            { 
                Name = "Cœur de Réseau National de Fibre Optique", 
                IpAddress = "197.230.10.1", 
                Domain = "backbone.ansut.ci", 
                Type = "Infrastructure", 
                Criticality = "Haute", 
                OrganizationId = ansut.Id, 
                DateRegistered = DateTime.Now.AddMonths(-4) 
            };
            var asset5 = new Asset 
            { 
                Name = "Base de Données Registre Médical Central", 
                IpAddress = "10.200.14.88", 
                Domain = "db-sante.local", 
                Type = "Database", 
                Criticality = "Haute", 
                OrganizationId = chu.Id, 
                DateRegistered = DateTime.Now.AddMonths(-3) 
            };
            var asset6 = new Asset 
            { 
                Name = "Serveur de Messagerie Ministérielle MNTD", 
                IpAddress = "160.155.3.15", 
                Domain = "mail.telecom.gouv.ci", 
                Type = "Email", 
                Criticality = "Moyenne", 
                OrganizationId = artci.Id, 
                DateRegistered = DateTime.Now.AddMonths(-5) 
            };

            context.Assets.AddRange(asset1, asset2, asset3, asset4, asset5, asset6);
            context.SaveChanges();

            // 5. Vulnerabilities
            var vulndef1 = new Vulnerability 
            { 
                CveId = "CVE-2026-1048", 
                Title = "Exécution de code à distance (RCE) via Apache Struts", 
                Description = "Une vulnérabilité critique d'injection permet l'exécution de commandes système arbitraires sans authentification.", 
                OwaspCategory = "A03:2021-Injection", 
                CvssScore = 9.8, 
                DateDetected = DateTime.Now.AddDays(-10) 
            };
            var vulndef2 = new Vulnerability 
            { 
                CveId = "CVE-2024-44228", 
                Title = "Log4Shell - Injection JNDI Apache Log4j", 
                Description = "Faille critique dans Log4j permettant la prise de contrôle complète du serveur distant.", 
                OwaspCategory = "A03:2021-Injection", 
                CvssScore = 10.0, 
                DateDetected = DateTime.Now.AddDays(-25) 
            };
            var vulndef3 = new Vulnerability 
            { 
                CveId = "CVE-2026-0812", 
                Title = "SQL Injection dans le module d'authentification e-Impôts", 
                Description = "Absence de vérification des paramètres SQL entraînant le contournement de la vérification du mot de passe.", 
                OwaspCategory = "A03:2021-Injection", 
                CvssScore = 9.1, 
                DateDetected = DateTime.Now.AddDays(-5) 
            };
            var vulndef4 = new Vulnerability 
            { 
                CveId = "CVE-2025-2140", 
                Title = "Défaut de gestion des jetons OAuth 2.0 (Broken Access Control)", 
                Description = "Contournement d'accès permettant d'usurper les privilèges d'administrateur système.", 
                OwaspCategory = "A01:2021-Broken Access Control", 
                CvssScore = 8.5, 
                DateDetected = DateTime.Now.AddDays(-14) 
            };
            var vulndef5 = new Vulnerability 
            { 
                CveId = "CVE-2025-4911", 
                Title = "Cross-Site Scripting (XSS) stocké dans le portail citoyen", 
                Description = "Possibilité d'injecter des scripts malveillants répercutés aux utilisateurs connectés.", 
                OwaspCategory = "A03:2021-Injection", 
                CvssScore = 6.4, 
                DateDetected = DateTime.Now.AddDays(-2) 
            };

            context.Vulnerabilities.AddRange(vulndef1, vulndef2, vulndef3, vulndef4, vulndef5);
            context.SaveChanges();

            // 6. Incidents (RTIR Tickets)
            var inc1 = new Incident 
            { 
                TicketNumber = "RTIR-2026-0428", 
                AssetId = asset1.Id, 
                VulnerabilityId = vulndef1.Id, 
                Status = "Notifié", 
                Severity = "Critique", 
                DateDetected = DateTime.Now.AddDays(-9), 
                DateTicketCreated = DateTime.Now.AddDays(-9).AddHours(2), 
                DateEmailSent = DateTime.Now.AddDays(-9).AddHours(4), 
                FollowUpNotes = "Notification automatique transmise à la cellule SOC ANSSI. Correctif logiciel en cours de test dans l'environnement de staging.",
                RemediationSteps = "Mettre à jour Apache Struts vers la version 2.5.33 minimum et restreindre les flux HTTP d'administration."
            };

            var inc2 = new Incident 
            { 
                TicketNumber = "RTIR-2026-0429", 
                AssetId = asset2.Id, 
                VulnerabilityId = vulndef3.Id, 
                Status = "Qualifié", 
                Severity = "Critique", 
                DateDetected = DateTime.Now.AddDays(-4), 
                DateTicketCreated = DateTime.Now.AddDays(-4).AddHours(1), 
                FollowUpNotes = "Corrélation ELK/IntelMQ confirmée. Alerte transmise à la DGI pour application immédiate du correctif de sécurité.",
                RemediationSteps = "Paramétrer des requêtes préparées (Prepared Statements) et activer le WAF gouvernemental."
            };

            var inc3 = new Incident 
            { 
                TicketNumber = "RTIR-2026-0430", 
                AssetId = asset3.Id, 
                VulnerabilityId = vulndef2.Id, 
                Status = "Clos", 
                Severity = "Critique", 
                DateDetected = DateTime.Now.AddDays(-20), 
                DateTicketCreated = DateTime.Now.AddDays(-20).AddHours(1), 
                DateEmailSent = DateTime.Now.AddDays(-20).AddHours(3), 
                DateVulnerabilityFixed = DateTime.Now.AddDays(-15), 
                DateClosed = DateTime.Now.AddDays(-14), 
                FollowUpNotes = "Correctif Log4j appliqué avec succès par l'équipe TrésorPay. Audit de suivi validé par le CI-CERT.",
                RemediationSteps = "Mise à jour du package log4j-core vers la v2.17.1. Scan de vulnérabilités post-fix négatif."
            };

            var inc4 = new Incident 
            { 
                TicketNumber = "RTIR-2026-0431", 
                AssetId = asset4.Id, 
                VulnerabilityId = vulndef4.Id, 
                Status = "Résolu", 
                Severity = "Haute", 
                DateDetected = DateTime.Now.AddDays(-12), 
                DateTicketCreated = DateTime.Now.AddDays(-12).AddHours(2), 
                DateEmailSent = DateTime.Now.AddDays(-12).AddHours(5), 
                DateVulnerabilityFixed = DateTime.Now.AddDays(-3), 
                FollowUpNotes = "Validation en cours avant clôture officielle du ticket RTIR par l'analyste principal.",
                RemediationSteps = "Revocation des anciens jetons d'accès OAuth et mise à niveau de la bibliothèque d'authentification."
            };

            var inc5 = new Incident 
            { 
                TicketNumber = "RTIR-2026-0432", 
                AssetId = asset5.Id, 
                VulnerabilityId = vulndef5.Id, 
                Status = "Détecté", 
                Severity = "Moyenne", 
                DateDetected = DateTime.Now.AddDays(-1), 
                DateTicketCreated = DateTime.Now.AddDays(-1).AddHours(1), 
                FollowUpNotes = "Détection automatique ELK enregistrée. Attente de la qualification technique par le CERT.",
                RemediationSteps = "Sanitisation des entrées formulaires et filtrage des balises HTML côté serveur."
            };

            context.Incidents.AddRange(inc1, inc2, inc3, inc4, inc5);
            context.SaveChanges();
        }
    }
}
