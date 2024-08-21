using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TranslationsBuilderTestClient.Services;
using Microsoft.AspNetCore.Hosting;

namespace AppOwnsData.Controllers {

  public class HomeController : Controller {

    private PowerBiServiceApi powerBiServiceApi;

    public HomeController(PowerBiServiceApi powerBiServiceApi) {
      this.powerBiServiceApi = powerBiServiceApi;
    }

    public IActionResult Index() {
      return View();
    }

  }
}
