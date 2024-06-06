using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TranslationsBuilderTestClient.Models;
using TranslationsBuilderTestClient.Services;

namespace TranslationsBuilderTestClient.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ViewModelController : ControllerBase {

    private PowerBiServiceApi powerBiServiceApi;

    public ViewModelController(PowerBiServiceApi powerBiServiceApi) {
      this.powerBiServiceApi = powerBiServiceApi;
    }

    public async Task<EmbeddingViewModel> GetViewModel() {
      return await powerBiServiceApi.GetEmbeddingViewModel();
    }


    }
}
