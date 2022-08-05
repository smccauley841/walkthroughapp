using System.Net;
using Microsoft.AspNetCore.Mvc;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.Choices;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Controllers
{
    public class ChoicesController : WalkthroughAppApiController
    {
        private readonly IGetChoices _getChoices;
        private readonly ICreateChoices _createChoices;
        private readonly IDelete<Choice> _deleteChoice;
        private readonly IUpdate<Choice> _updateChoice;

        public ChoicesController(IGetChoices getChoices,
                                 ICreateChoices createChoices,
                                 IDelete<Choice> deleteChoice,
                                 IUpdate<Choice> updateChoice)
        {
            _getChoices = getChoices;
            _createChoices = createChoices;
            _deleteChoice = deleteChoice;
            _updateChoice = updateChoice;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var choices = await _getChoices.GetAll();
            if (choices.Count() == 0)
                return NotFound("No choices were found");
            return Ok(choices);
        }
        [HttpGet("{choiceId}")]
        public async Task<IActionResult> GetById(int choiceId)
        {
            var choice = _getChoices.GetById(choiceId);

            if (choice == null)
                return NotFound("No job title was found");

            return Ok(choice);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestionChoices([FromBody]NewChoice[] newChoices)
        {
            foreach (var newChoice in newChoices)
            {
                if (string.IsNullOrWhiteSpace(newChoice.ChoiceText))
                    return BadRequest("Choice text must not be null or empty on any choice");
                if (await _createChoices.DoesQuestionChoiceExist(newChoice))
                    return StatusCode((int)HttpStatusCode.Conflict, $"Choice with text: {newChoice.ChoiceText} already exists for this question");
                if (await _createChoices.DoesQuestionAlreadyHaveACorrectAnswer(newChoice) && newChoice.IsAnswer)
                    return StatusCode((int)HttpStatusCode.Conflict, $"A correct answer already exists for this question");
            }
            var choices = await _createChoices.CreateQuestionChoices(newChoices);
            return Created("",choices);
        }

        [HttpGet("questionId")]
        public async Task<IActionResult> GetByQuestionId(int id)
        {
            var choices = await _getChoices.GetByQuestionId(id);
            return Ok(choices);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChoice(int deletedChoice)
        {
            var choice = _getChoices.GetById(deletedChoice);

            if (choice == null)
                return NotFound("Choice does not exist");

            await _deleteChoice.Delete(choice);

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateChoice(Choice updatedChoice)
        {
            var choice = _getChoices.GetById(updatedChoice.Id);
            if (choice == null)
                return NotFound("Choice does not exist");

            choice.ChoiceText = updatedChoice.ChoiceText;

            if(updatedChoice.IsAnswer != null)
                choice.IsAnswer = updatedChoice.IsAnswer;

            await _updateChoice.UpdateItem(choice);

            return Ok(choice);
        }
    }
}
