using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using TranslationsBuilderTestClient.Models;

namespace TranslationsBuilderTestClient.Services {

  public class PowerBiServiceApi {

    private ITokenAcquisition tokenAcquisition { get; }
    private Guid workspaceId { get; }
    private Guid reportId { get; }
    private string powerbiServiceApiRoot { get; }
    private string powerBiServiceApiResourceId { get; }
    public string powerbiDefaultScope { get; }

    public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition) {

      // get configuration settings
      this.powerbiServiceApiRoot = configuration["PowerBi:PowerBiServiceApiRoot"];
      this.powerBiServiceApiResourceId = configuration["PowerBi:PowerBiServiceApiResourceId"];
      this.powerbiDefaultScope = this.powerBiServiceApiResourceId + "/.default";
      this.workspaceId = Guid.Parse(configuration["PowerBi:workspaceId"]);
      this.reportId = Guid.Parse(configuration["PowerBi:reportId"]);

      // get reference to token acquisition service
      this.tokenAcquisition = tokenAcquisition;
    }

    public string GetAppOnlyAccessToken() {
      return this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiDefaultScope).Result;
    }

    public PowerBIClient GetPowerBiClient() {
      var tokenCredentials = new TokenCredentials(GetAppOnlyAccessToken(), "Bearer");
      return new PowerBIClient(new Uri(powerbiServiceApiRoot), tokenCredentials);
    }

    public async Task<EmbeddingViewModel> GetEmbeddingViewModel() {

      PowerBIClient pbiClient = GetPowerBiClient();

      var report = await pbiClient.Reports.GetReportInGroupAsync(workspaceId, reportId);

      // create token request object
      GenerateTokenRequest generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "View");

      // call to Power BI Service API and pass GenerateTokenRequest object to generate embed token
      string embedToken = pbiClient.Reports.GenerateTokenInGroup(workspaceId, report.Id,
                                                                 generateTokenRequestParameters).Token;

      // return report embedding data to caller
      return new EmbeddingViewModel {
        ReportId = reportId.ToString(),
        EmbedUrl = report.EmbedUrl,
        Token = embedToken,
        Langauges = SupportedLanguages.AllLangauges   
      };
    }

  }
}
