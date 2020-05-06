TODO

# Scenario: Create a report on which apps and versions customers are running + which apps have updates available from App Source
#   - APIs used
# get customers
# get environments
# get apps installed
# get available updates
# smtp

# Scenario: Export all customer environments to bacpacs
#   - APIs used:
#       Partner Center API:   GET  customers
#       Business Central API: GET  https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments
#       Business Central API: POST https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/<environmentName>/Export
#       Business Central API: GET  https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/<environmentName>/ExportSTatus
#       SMTP API
StartExportForAllCustomerEnvironments
MonitorOngoingExports
SendSummaryEmail

