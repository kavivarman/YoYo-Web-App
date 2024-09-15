using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoYo_Web_App.Models.Yoyo;

namespace YoYo_Web_App.Controllers
{
    /// <summary>
    /// The Yoyo Controller
    /// Contains all action methods
    /// </summary>
    public class YoyoController : Controller
    {
        private readonly IYoyoRepository _yoyoRepository;

        /// <summary>
        /// The yoyo controller constructor.
        /// </summary>
        /// <param name="yoyoRepository">Contains yoyo repository object</param>
        public YoyoController(IYoyoRepository yoyoRepository)
        {
            _yoyoRepository = yoyoRepository;
        }
        /// <summary>
        /// This method called, when yoyo page is loaded/refreshed.
        /// </summary>
        /// <returns>yoyo view with list of athletes</returns>
        [HttpGet]
        public async Task<IActionResult> YoyoView()
        {
            var yoyoViewModel = await _yoyoRepository.GetViewModelAsync();
            if (yoyoViewModel != null)
                return View(yoyoViewModel);
            return View("~/Views/Yoyo/NoTestFoundView.cshtml");
        }
        /// <summary>
        /// This method called, when yoyo page is loaded/refreshed.
        /// </summary>
        /// <returns>list of beep test data</returns>
        [HttpGet]
        public async Task<IActionResult> GetBeepTestData()
        {
            var beepTestData = await _yoyoRepository.GetBeepTestData();
            if (beepTestData != null)
                return new JsonResult(beepTestData);
            return NotFound("Beep test file might be empty or not found");
        }
        /// <summary>
        /// This method called, when coach warned or stopped the athletes and updates 
        /// level, shuttle details of athletes respectively. It also called when coach changes the level, shuttle of stopped athletes
        /// </summary>
        /// <param name="id">The unique athlete id</param>
        /// <param name="actionType">Enum completed(stopped)/warned</param>
        /// <param name="levelShuttleDetail">Contains completed(stopped)/warned athletes level and shuttle details.</param>
        /// <returns>successfully updated stopped/warned athletesInfo </returns>
        [HttpPut]
        public IActionResult UpdateAthleteStatus(int id, ActionType actionType, [FromBody] LevelShuttleDetail levelShuttleDetail)
        {
            if (levelShuttleDetail == null)
                return BadRequest($"Invalid object value - levelShuttleDetail");

            var response = _yoyoRepository.UpdateAthleteStatus(id, actionType, levelShuttleDetail);

            if (response != null)
                return Ok(response);
            return NotFound($"Athlete id {id} is not found");
        }
        /// <summary>
        /// This method called, when shuttle time is over,it updates the last completed shuttle, level details of active athletes
        /// </summary>
        /// <param name="levelShuttleDetails">Contains list of athletes with completed level and shuttle details</param>
        /// <returns>successfully updated list of completed(stopped)/warned athletes</returns>
        [HttpPut]
        public IActionResult OnTestCompleted([FromBody] IEnumerable<LevelShuttleDetails> levelShuttleDetails)
        {
            if (levelShuttleDetails == null)
                return BadRequest($"Invalid object value - levelShuttleDetails");

            var response = _yoyoRepository.OnTestCompleted(levelShuttleDetails);

            if (!response.IsValid)
                return NotFound($"param - levelShuttleDetails might be null or  one or more athlete Id is invalid");
            return Ok(response);
        }
        /// <summary>
        /// This method called, when view result button is clicked
        /// </summary>
        /// <returns>List of athletes with updated stopped(completed)/warned details.</returns>
        [HttpGet]
        public IActionResult ViewResult()
        {
            var response = _yoyoRepository.ViewResult();
            if (response != null)
                return Ok(response);
            return NotFound($"Test result not found");
        }
    }
}
