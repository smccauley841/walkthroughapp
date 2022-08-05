using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.Walkthroughs;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Controllers
{
    public class WalkthroughController : WalkthroughAppApiController
    {
        private readonly IGet<Walkthrough> _getWalkthroughs;
        private readonly ICreate<Walkthrough, NewWalkthrough> _createWalkthrough;
        private readonly IDelete<Walkthrough> _deleteWalkthrough;
        private readonly IUpdate<Walkthrough> _updateWalkthrough;

        public WalkthroughController(IGet<Walkthrough> getWalkthroughs,
                                     ICreate<Walkthrough, NewWalkthrough> createWalkthrough,
                                     IDelete<Walkthrough> deleteWalkthrough,
                                     IUpdate<Walkthrough> updateWalkthrough)
        {
            _getWalkthroughs = getWalkthroughs;
            _createWalkthrough = createWalkthrough;
            _deleteWalkthrough = deleteWalkthrough;
            _updateWalkthrough = updateWalkthrough;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var walkthroughs = await _getWalkthroughs.GetAll();
            if(walkthroughs.Count() == 0)
                return NotFound("No walkthroughs were found");
            return Ok(walkthroughs);
        }

        [HttpGet("{walkthroughId}")]
        public IActionResult GetById(int walkthroughId)
        {
            var walkthrough = _getWalkthroughs.GetById(walkthroughId);

            if (walkthrough == null)
                return NotFound("No walkthrough was found");

            return Ok(walkthrough);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewWalkthrough([FromBody] NewWalkthrough newWalkthrough)
        {
            if (string.IsNullOrWhiteSpace(newWalkthrough.WalkthroughName))
                return BadRequest("Walkthrough name must not be null or empty");

            if(_createWalkthrough.DoesItemExist(newWalkthrough))
                return StatusCode((int)HttpStatusCode.Conflict, $"Walkthrough with name '{newWalkthrough.WalkthroughName}' already exists for this job role.");

            return Created("", _createWalkthrough.Create(newWalkthrough));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWalkthrough(int deletedWalkthrough)
        {
            var walkthrough = _getWalkthroughs.GetById(deletedWalkthrough);

            if (walkthrough == null)
                return NotFound("Walkthrough does not exist");

            await _deleteWalkthrough.Delete(walkthrough);

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateWalkthrough(Walkthrough updatedWalkthrough)
        {
            var walkthrough = _getWalkthroughs.GetById(updatedWalkthrough.Id);
            if (walkthrough == null)
                return NotFound("Walkthrough does not exist");

            walkthrough.WalkthroughName = updatedWalkthrough.WalkthroughName;

            await _updateWalkthrough.UpdateItem(walkthrough);

            return Ok(walkthrough);
            

        }
    }
}
