using api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KeyBindingController : ControllerBase
    {
        private readonly ILogger<KeyBindingController> _logger;
        private readonly AppDbContext _appDbContext;

        public KeyBindingController(AppDbContext appDbContext, ILogger<KeyBindingController> logger)
        {
            this._appDbContext = appDbContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetKeyBindings")]
        public ActionResult<string> GetKeyBindings()
        {
            var item = _appDbContext.KeyBindings.ToList();
            return Ok(item);
        }

        [HttpGet]
        [Route("GetKeyBinding")]
        public ActionResult<string> GetKeyBinding(int id)
        {
            var item = _appDbContext.KeyBindings.FirstOrDefault(c => c.id == id);
            return Ok(item);
        }

        [HttpPost]
        [Route("AddKeyBinding")]
        public ActionResult AddKeyBinding([FromBody] KeyBinding item)
        {
            _appDbContext.KeyBindings.Add(item);
            _appDbContext.SaveChanges();
            return CreatedAtAction(nameof(GetKeyBinding), new { id = item.id }, item);
        }

        [HttpPost]
        [Route("UpdateKeyBinding")]
        public ActionResult UpdateKeyBinding(KeyBinding item)
        {
            var findItem = _appDbContext.KeyBindings.FirstOrDefault(c => c.id == item.id);
            if (findItem == null)
            {
                return NotFound();
            }
            findItem.Action = item.Action;
            findItem.Category = item.Category;
            findItem.Binding = item.Binding;
            _appDbContext.KeyBindings.Update(findItem);
            _appDbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        [Route("DeleteKeyBinding")]
        public ActionResult DeleteKeyBinding(int id)
        {
            var item = _appDbContext.KeyBindings.FirstOrDefault(c => c.id == id);
            if (item == null)
            {
                return NotFound();
            }
            _appDbContext.KeyBindings.Remove(item);
            _appDbContext.SaveChanges();
            return NoContent();
        }


    }
}
