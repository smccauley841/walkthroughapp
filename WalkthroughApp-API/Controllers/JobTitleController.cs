using System.Net;
using Microsoft.AspNetCore.Mvc;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Controllers
{
    public class JobTitleController : WalkthroughAppApiController
    {
        private readonly IGet<JobTitle> _getJobTitles;
        private readonly ICreate<JobTitle, NewJobTitle> _createJobTitle;
        private readonly IDelete<JobTitle> _deleteJobTitle;
        private readonly IUpdate<JobTitle> _updateJobTitle;

        public JobTitleController(IGet<JobTitle> getJobTitles,
                                  ICreate<JobTitle, NewJobTitle> createJobTitle,
                                  IDelete<JobTitle> deleteJobTitle,
                                  IUpdate<JobTitle> updateJobTitle)
        {
            _getJobTitles = getJobTitles;
            _createJobTitle = createJobTitle;
            _deleteJobTitle = deleteJobTitle;
            _updateJobTitle = updateJobTitle;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobTitles = await _getJobTitles.GetAll();
            if (jobTitles.Count() == 0)
                return NotFound("No job titles were found");
            return Ok(jobTitles);
        }
        [HttpGet("{jobTitleId}")]
        public async Task<IActionResult> GetById(int jobTitleId)
        {
            var jobTitle = _getJobTitles.GetById(jobTitleId);

            if (jobTitle == null)
                return NotFound("No job title was found");

            return Ok(jobTitle);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewJobTitle([FromBody] NewJobTitle newJobTitle)
        {
            if (string.IsNullOrWhiteSpace(newJobTitle.Name))
                return BadRequest("Job title name must not be null or empty");

            if (_createJobTitle.DoesItemExist(newJobTitle))
                return StatusCode((int)HttpStatusCode.Conflict, $"Job with name '{newJobTitle.Name}' already exists.");

            return Created("", _createJobTitle.Create(newJobTitle));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteJob(int deletedJob)
        {
            var jobTitle = _getJobTitles.GetById(deletedJob);

            if (jobTitle == null)
                return NotFound("Job does not exist");

            await _deleteJobTitle.Delete(jobTitle);

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateJobTitle(JobTitle updateJobTitle)
        {
            var job = _getJobTitles.GetById(updateJobTitle.Id);
            if (job == null)
                return NotFound("Job does not exist");

            job.Name = updateJobTitle.Name;

            await _updateJobTitle.UpdateItem(job);

            return Ok(job);
        }
    }
}
